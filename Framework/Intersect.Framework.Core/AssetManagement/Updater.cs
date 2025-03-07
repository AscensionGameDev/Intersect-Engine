using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Intersect.Core;
using Intersect.Web;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace Intersect.Framework.Core.AssetManagement;

public enum UpdaterStatus
{
    Offline,
    NeedsAuthentication,
    Ready,
    NoUpdateNeeded,
}

public sealed class Updater
{
    public const string KeywordAppManifest = "#appmanifest";

    private const long FailedRetryLimit = 5;
    internal const int MaxBuffer = 1024 * 1024;

    private static readonly string[] SizeSuffixes = ["B", "KiB", "MiB", "GiB", "TiB"];

    private readonly ConcurrentQueue<UpdateManifestFile> _downloadQueue = new();
    private readonly ConcurrentDictionary<UpdateManifestFile, DownloadResult> _downloadResults = new();
    private readonly DirectoryInfo _updateRootInfo = new(Path.GetFullPath(".", Environment.CurrentDirectory));
    private readonly string _versionPath;
    private readonly string? _baseUrl;
    private readonly string? _manifestUrl;
    private readonly Thread? _mainThread;
    private readonly int _workerThreadCount = 1;

    private Thread[] _workerThreads;

    private ulong _activeThreads;
    private long _downloadedBytes;
    private bool _failed;
    private bool _stopping;
    private bool _updaterContentChanged;

    public Updater(string manifestUrl, string applicationManifestFileName, string currentVersionPath, int maxDownloadThreads = 10)
    {
        if (string.IsNullOrWhiteSpace(manifestUrl))
        {
            return;
        }

        _workerThreadCount = maxDownloadThreads;
        _versionPath = currentVersionPath;
        _mainThread = new Thread(RunSynchronously);

        if (manifestUrl.EndsWith('/'))
        {
            _baseUrl = manifestUrl;
        }

        var sanitizedManifestUrl = manifestUrl.TrimEnd('/');

        if (sanitizedManifestUrl.EndsWith(KeywordAppManifest))
        {
            sanitizedManifestUrl = sanitizedManifestUrl.Replace(KeywordAppManifest, applicationManifestFileName);
        }

        _manifestUrl = sanitizedManifestUrl;

        if (_baseUrl != default)
        {
            return;
        }

        Uri uri = new(sanitizedManifestUrl);
        var includedSegments = uri.Segments.Take(Math.Max(0, uri.Segments.Length - 1))
            .Select(segment => segment.TrimEnd('/'))
            .Where(segment => !string.IsNullOrWhiteSpace(segment))
            .ToArray();
        UriBuilder uriBuilder = new(uri)
        {
            Path = string.Join('/', includedSegments),
        };
        uri = uriBuilder.Uri;
        _baseUrl = uri.AbsoluteUri.TrimEnd('/');
    }

    private UpdateManifest? _fetchedManifest;
    private UpdaterStatus _updaterStatus;
    private TokenResponse? _tokenResponse;

    public void SetAuthorizationData(TokenResponse tokenResponse)
    {
        _tokenResponse = tokenResponse;
    }

    public UpdaterStatus TryGetManifest(out UpdateManifest? updateManifest, int reattemptTimes = 0, bool force = false)
    {
        if (!force)
        {
            if (string.IsNullOrWhiteSpace(_manifestUrl))
            {
                updateManifest = default;
                return UpdaterStatus.NoUpdateNeeded;
            }

            if (_fetchedManifest != default)
            {
                updateManifest = _fetchedManifest;
                return _updaterStatus;
            }
        }

        try
        {
            using IntersectHttpClient httpClient = new(_baseUrl, _tokenResponse);

            HttpResponseMessage? responseMessage = default;

            var manifestRequestFailures = new Exception[1 + reattemptTimes];

            for (var attempt = 0; attempt < manifestRequestFailures.Length; ++attempt)
            {
                try
                {
                    using HttpRequestMessage requestMessage = new(
                        HttpMethod.Get,
                        $"{_manifestUrl}?token={Environment.TickCount}"
                    );

                    ApplicationContext.CurrentContext.Logger.LogInformation(
                        "Attempting to fetch update manifest from {ManifestUrl}",
                        _manifestUrl
                    );

                    responseMessage = httpClient.Send(requestMessage);

                    switch (responseMessage.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            updateManifest = default;
                            return UpdaterStatus.NeedsAuthentication;
                        case >= HttpStatusCode.BadRequest:
                            continue;
                    }

                    break;
                }
                catch (Exception exception)
                {
                    manifestRequestFailures[attempt] = exception;

                    Thread.Sleep(5000);
                }
            }

            if (responseMessage == default)
            {
                throw new AggregateException(manifestRequestFailures);
            }

            // Can't do a block-less using without a declaration
            using var _ = responseMessage;

            using var responseMessageStream = responseMessage.Content.ReadAsStream();
            using StreamReader streamReader = new(responseMessageStream);
            var rawManifest = streamReader.ReadToEnd();
            updateManifest = JsonConvert.DeserializeObject<UpdateManifest>(rawManifest);

            if (updateManifest == null)
            {
                throw new NullReferenceException("No update manifest from remote server");
            }

            var updateRequired = !updateManifest.TrustCache;

            var cachedManifestFilePath = Path.GetFullPath(_versionPath, _updateRootInfo.FullName);
            FileInfo cachedManifestFileInfo = new(cachedManifestFilePath);

            // ReSharper disable once InvertIf
            if (!updateRequired)
            {
                if (cachedManifestFileInfo.Exists)
                {
                    var updateFileLookup = updateManifest.Files.ToDictionary(umf => umf.Path, umf => umf);
                    try
                    {
                        var rawCachedVersion = File.ReadAllText(cachedManifestFileInfo.FullName);
                        var cachedManifest = JsonConvert.DeserializeObject<UpdateManifest>(rawCachedVersion);
                        if (cachedManifest == default)
                        {
                            cachedManifestFileInfo.Delete();
                        }
                        else
                        {
                            updateRequired = false;
                            var cacheFileLookup = cachedManifest.Files.ToDictionary(umf => umf.Path, umf => umf);

                            foreach (var updateFile in updateManifest.Files)
                            {
                                if (!cacheFileLookup.TryGetValue(updateFile.Path, out var cachedFile))
                                {
                                    updateRequired = true;
                                    break;
                                }

                                if (cachedFile.Size != updateFile.Size)
                                {
                                    updateRequired = true;
                                    break;
                                }

                                updateRequired = !string.Equals(
                                    updateFile.Checksum,
                                    cachedFile.Checksum,
                                    StringComparison.Ordinal
                                );

                                if (updateRequired)
                                {
                                    break;
                                }
                            }

                            var cachedFiles = cachedManifest.Files.Where(f => !updateFileLookup.ContainsKey(f.Path));
                            foreach (var cachedFile in cachedFiles)
                            {
                                var resolvedPath = ResolvePath(cachedFile.Path, _updateRootInfo.FullName);
                                if (!File.Exists(resolvedPath))
                                {
                                    continue;
                                }

                                try
                                {
                                    File.Delete(resolvedPath);
                                }
                                catch (Exception exception)
                                {
                                    ApplicationContext.CurrentContext.Logger.LogWarning(
                                        exception,
                                        "Failed to delete cached file: \"{CacheFilePath}\"",
                                        cachedFile.Path
                                    );
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        ApplicationContext.CurrentContext.Logger.LogWarning(
                            exception,
                            "Error inspecting existing manifest, forcing update"
                        );
                        updateRequired = true;
                    }
                }
                else
                {
                    updateRequired = true;
                }
            }

            _fetchedManifest = updateManifest;
            _updaterStatus = !updateRequired ? UpdaterStatus.NoUpdateNeeded : UpdaterStatus.Ready;
            return _updaterStatus;
        }
        catch (Exception exception)
        {
            ApplicationContext.CurrentContext.Logger.LogError(exception, "Failed to access update server");
            updateManifest = default;
            return UpdaterStatus.Offline;
        }
    }

    public Task Start(CancellationToken cancellationToken = default)
    {
        if (_mainThread == default)
        {
            return Task.CompletedTask;
        }

        return Task.Run(
            () =>
            {
                _mainThread.Start(cancellationToken);
                _mainThread.Join();
            },
            cancellationToken
        );
    }

    private void RunSynchronously(object? boxedCancellationToken)
    {
        var cancellationToken = boxedCancellationToken is CancellationToken token ? token : default;
        RunUpdates(cancellationToken);
    }

    public long BytesDownloaded => _downloadedBytes +
                                   _downloadResults.Sum(
                                       pair => pair.Value.State == DownloadState.Active
                                           ? pair.Value.Count
                                           : 0
                                   );

    public Exception Exception { get; private set; }

    public int FilesRemaining =>
        _downloadQueue.Count + _downloadResults.Count(p => p.Value.State == DownloadState.Active);

    public float Progress => (BytesDownloaded / (float)SizeTotal) * 100f;

    public bool ShouldRestart { get; private set; }

    public long SizeRemaining => SizeTotal - BytesDownloaded;

    private long _sizeTotal;

    public long SizeTotal
    {
        get
        {
#if DIAGNOSTIC
            ApplicationContext.CurrentContext.Logger.LogTrace($"Reading SizeTotal {Environment.TickCount64}");
#endif
            return _sizeTotal;
        }
    }

    public UpdateStatus Status { get; private set; } = UpdateStatus.DownloadingManifest;

    private void RunUpdates(CancellationToken cancellationToken)
    {
        DeleteTemporaryFiles();

        UpdateManifest updateManifest;
        UpdateManifest assetManifest;

        try
        {
            var manifestStatus = TryGetManifest(out updateManifest, 5);
            switch (manifestStatus)
            {
                case UpdaterStatus.Offline:
                    throw new Exception("Failed to download update manifest");
                case UpdaterStatus.NoUpdateNeeded:
                    Status = UpdateStatus.None;
                    ApplicationContext.CurrentContext.Logger.LogInformation("Assets are already up-to-date");
                    return;
                case UpdaterStatus.Ready:
                    break;
                case UpdaterStatus.NeedsAuthentication:
                    throw new Exception("Not authenticated");
                default:
                    throw new UnreachableException();
            }

            assetManifest = new UpdateManifest
            {
                StreamingUrl = updateManifest.StreamingUrl,
                TrustCache = updateManifest.TrustCache,
            };

            List<UpdateManifestFile> resourcesToDownload = [];

            // The alternative code is super unreadable and *ugly
            // ReSharper didn't stop to think if it should, only if it could
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var file in updateManifest.Files)
            {
                var resolvedPath = ResolvePath(file.Path, _updateRootInfo.FullName);
                FileInfo fileInfo = new(resolvedPath);

                var stale = !fileInfo.Exists ||
                            file.Size != fileInfo.Length ||
                            file.Checksum != UpdateManifestFile.ComputeChecksum(fileInfo);
                if (!stale)
                {
                    assetManifest.Files.Add(file);
                    continue;
                }

                if (IsUpdaterFile(file.Path))
                {
                    EnqueueLocked(file);
#if DIAGNOSTIC
                    ApplicationContext.CurrentContext.Logger.LogTrace($"Executing Interlocked.Add(ref _sizeTotal ({_sizeTotal}), {file.Size})");
#endif
                    Interlocked.Add(ref _sizeTotal, file.Size);
#if DIAGNOSTIC
                    ApplicationContext.CurrentContext.Logger.LogTrace($"Done Interlocked.Add(ref _sizeTotal ({_sizeTotal}), {file.Size})");
#endif
                }

                resourcesToDownload.Add(file);
            }

            foreach (var file in resourcesToDownload)
            {
                if (!_downloadQueue.Contains(file))
                {
                }

                var downloadResult = new DownloadResult
                {
                    OverrideBaseUrl = updateManifest.OverrideBaseUrl,
                    State = DownloadState.Queued,
                };

                if (!_downloadResults.TryAdd(file, downloadResult))
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning(
                        "Failed to add file to download results, it may already be registered? {OutputFilePath}",
                        file.Path
                    );
                }

                if (IsUpdaterFile(file.Path))
                {
                    continue;
                }

                EnqueueLocked(file);
#if DIAGNOSTIC
                ApplicationContext.CurrentContext.Logger.LogTrace($"Executing Interlocked.Add(ref _sizeTotal ({_sizeTotal}), {file.Size})");
#endif
                Interlocked.Add(ref _sizeTotal, file.Size);
#if DIAGNOSTIC
                ApplicationContext.CurrentContext.Logger.LogTrace($"Done Interlocked.Add(ref _sizeTotal ({_sizeTotal}), {file.Size})");
#endif
            }

            if (_downloadQueue.IsEmpty)
            {
                Status = UpdateStatus.None;

                return;
            }

            Status = UpdateStatus.UpdateInProgress;
        }
        catch (Exception exception)
        {
            // Failed to fetch update info or deserialize!
            Status = UpdateStatus.Error;
            Exception = new Exception($"[Update Check Failed!] - {exception.Message}", exception);
            ApplicationContext.CurrentContext.Logger.LogError(exception, "Update check failed");
            return;
        }

        Status = UpdateStatus.UpdateInProgress;

        var streamingSuccess = false;

        if (!string.IsNullOrWhiteSpace(updateManifest.StreamingUrl))
        {
            streamingSuccess = StreamDownloads(updateManifest, cancellationToken);
        }

        if (!streamingSuccess)
        {
            // ProcessorCount = # of logical threads
            // We need at least 1 thread, but there are 2 reserved threads:
            // - main updater thread (so we can control the other threads)
            // - main client thread (so the user gets feedback)
            var threadCount = Math.Clamp(
                Math.Min(_workerThreadCount, _downloadQueue.Count),
                1,
                Environment.ProcessorCount - 2
            );
            _workerThreads = new Thread[threadCount];

            for (int workerThreadIndex = 0; workerThreadIndex < threadCount; workerThreadIndex++)
            {
                _workerThreads[workerThreadIndex] = new Thread(
                    () =>
                    {
                        Interlocked.Increment(ref _activeThreads);
                        _failed |= !DownloadUpdates(workerThreadIndex, cancellationToken);
                        Interlocked.Decrement(ref _activeThreads);
                    }
                );
                _workerThreads[workerThreadIndex].Start();
            }
        }

        foreach (var thread in _workerThreads)
        {
            thread.Join();
        }

        if (_failed)
        {
            _failed = true;
            Status = UpdateStatus.Error;
            return;
        }

        lock (assetManifest)
        {
            ApplicationContext.CurrentContext.Logger.LogInformation("Collating downloads...");

            lock (_downloadResults)
            {
                foreach (var (file, result) in _downloadResults)
                {
                    if (result.State == DownloadState.Completed)
                    {
                        ApplicationContext.CurrentContext.Logger.LogInformation(
                            "Completed download : {OutputFilePath}",
                            file.Path
                        );
                        assetManifest.Files.Add(file);
                    }
                    else
                    {
                        ApplicationContext.CurrentContext.Logger.LogWarning(
                            "Download marked as {ActualDownloadState} instead of {ExpectedDownloadState}: {OutputFilePath}",
                            file.Path,
                            result.State,
                            DownloadState.Completed
                        );
                    }
                }
            }

            ApplicationContext.CurrentContext.Logger.LogInformation(
                "Finished downloads, now writing updated version manifest..."
            );

            File.WriteAllText(
                _versionPath,
                JsonConvert.SerializeObject(
                    assetManifest,
#if DEBUG
                    Formatting.Indented,
#else
                        Formatting.None,
#endif
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore }
                )
            );

            ApplicationContext.CurrentContext.Logger.LogInformation("Finished writing updated version manifest...");
        }


        if (_stopping)
        {
            return;
        }

        Status = ShouldRestart ? UpdateStatus.Restart : UpdateStatus.UpdateCompleted;
    }

    private bool StreamDownloads(UpdateManifest updateManifest, CancellationToken cancellationToken = default)
    {
        using IntersectHttpClient httpClient = new(_baseUrl, _tokenResponse);

        List<UpdateManifestFile> filesToStream = [];
        List<UpdateManifestFile> filesToRequeue = [];

        while (!_downloadQueue.IsEmpty && TryDequeueLocked(out var currentFile))
        {
            if (!_downloadResults.TryGetValue(currentFile, out _))
            {
                ApplicationContext.CurrentContext.Logger.LogWarning(
                    "Skipping download of unregistered file in {ManifestPropertyName}: {FilePath}",
                    nameof(UpdateManifest.OverrideBaseUrl),
                    currentFile.Path
                );
                _downloadResults[currentFile] = new DownloadResult { State = DownloadState.Queued };
            }

            if (currentFile.Size < updateManifest.StreamingSizeCutoff || IsUpdaterFile(currentFile.Path))
            {
                filesToStream.Add(currentFile);
                continue;
            }

            filesToRequeue.Add(currentFile);
        }

        foreach (var fileToRequeue in filesToRequeue)
        {
            EnqueueLocked(fileToRequeue);
        }

        var streamingUrl = $"{updateManifest.StreamingUrl}?token={Environment.TickCount}";
        Uri requestUri;
        try
        {
            requestUri = new Uri(streamingUrl);
            if (requestUri.Scheme.StartsWith("file"))
            {
                throw new InvalidOperationException("Cannot stream from a file: URI");
            }
        }
        catch (Exception exception)
        {
            if (_baseUrl == default)
            {
                ApplicationContext.CurrentContext.Logger.LogWarning(exception, "No base URL");
                return false;
            }

            try
            {
                Uri baseUri = new(_baseUrl);
                requestUri = new Uri(baseUri, streamingUrl);
            }
            catch (Exception secondException)
            {
                AggregateException aggregateException = new(exception, secondException);
                ApplicationContext.CurrentContext.Logger.LogWarning(
                    aggregateException,
                    "Unable to resolve streaming url: {StreamingURL}",
                    streamingUrl
                );
                return false;
            }
        }

        try
        {
            using HttpRequestMessage request = new(HttpMethod.Post, requestUri.AbsoluteUri);

            var requestPayload = filesToStream.Select(f => f.Path).ToArray();
            var serializedPayload = JsonConvert.SerializeObject(requestPayload);
            request.Content = new StringContent(serializedPayload, Encoding.UTF8, "application/json");

            using var responseMessage = httpClient.Send(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken
            );

            if (responseMessage.StatusCode == HttpStatusCode.PartialContent)
            {
                // TODO: Support 206 Partial Content
                return false;
            }

            using var responseStream = responseMessage.Content.ReadAsStream(cancellationToken);
            using var responseReader = new BinaryReader(responseStream);

            const int maxBuffer = 1024 * 1024;
            var buffer = new byte[maxBuffer];
            // ReSharper disable line TooWideLocalVariableScope
            int bufferOffset, chunkSize, read;
            long remaining = responseMessage.Content.Headers.ContentLength ?? 0;
            while (!cancellationToken.IsCancellationRequested && remaining > 0)
            {
                bufferOffset = 0;

                string assetName;
                try
                {
                    var assetNameLengthBuffer = responseReader.ReadBytes(sizeof(int));
                    remaining -= assetNameLengthBuffer.Length;

                    var assetNameLength = BinaryPrimitives.ReadInt32LittleEndian(assetNameLengthBuffer);
                    var assetNameBytes = responseReader.ReadBytes(assetNameLength);
                    remaining -= assetNameLength;

                    assetName = Encoding.UTF8.GetString(assetNameBytes);
                }
                // Keep this for debugging
                // ReSharper disable once RedundantCatchClause
                catch
                {
                    throw;
                }

                var currentFile = _downloadResults.Keys.FirstOrDefault(key => string.Equals(key.Path, assetName));
                if (currentFile == default)
                {
                    ApplicationContext.CurrentContext.Logger.LogError(
                        $"Streaming server returned unknown file, falling back to per-file downloads: {assetName}"
                    );
                    return false;
                }

                if (!_downloadResults.TryGetValue(currentFile, out var currentResult))
                {
                    ApplicationContext.CurrentContext.Logger.LogError($"Unregistered file, falling back to per-file downloads: {assetName}");
                    return false;
                }

                if (currentResult.State != DownloadState.Queued)
                {
                    ApplicationContext.CurrentContext.Logger.LogError(
                        $"Another thread is already downloading this, falling back to per-file downloads: {assetName}"
                    );
                    return false;
                }

                filesToStream.Remove(currentFile);

                var sizeBuffer = responseReader.ReadBytes(sizeof(long));
                var size = (int)BinaryPrimitives.ReadInt64LittleEndian(sizeBuffer);
                remaining -= sizeBuffer.Length;

                var resolvedPath = ResolvePath(currentFile.Path, _updateRootInfo.FullName);
                FileInfo targetFileInfo = new(resolvedPath);
                FileInfo temporaryFileInfo = new($"{targetFileInfo}.tmp");

                var targetDirectoryInfo = targetFileInfo.Directory;
                if (targetDirectoryInfo is { Exists: false })
                {
                    targetDirectoryInfo.Create();
                }

                using FileStream fileStream = temporaryFileInfo.OpenWrite();

                while (currentResult.Count < size)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    chunkSize = (int)Math.Min(
                        Math.Min(size - currentResult.Count, buffer.Length),
                        maxBuffer - bufferOffset
                    );

                    read = responseReader.Read(buffer, bufferOffset, chunkSize);
                    remaining -= read;

                    bufferOffset += read;
                    currentResult.Count += read;

                    if (bufferOffset < maxBuffer)
                    {
                        continue;
                    }

                    fileStream.Write(buffer, 0, bufferOffset);
                    bufferOffset = 0;
                }

                if (bufferOffset > 0)
                {
                    fileStream.Write(buffer, 0, bufferOffset);
                }

                fileStream.Close();

                var validity = ValidateTemporaryFile(currentFile, temporaryFileInfo, out var temporaryChecksum);

                var relativePathToTemporaryFile = Path.GetRelativePath(
                    _updateRootInfo.FullName,
                    temporaryFileInfo.FullName
                );

                var relativePathToTargetFile = Path.GetRelativePath(_updateRootInfo.FullName, targetFileInfo.FullName);

                if (validity != DownloadValidity.Valid)
                {
                    Exception = new InvalidDataException(
                        validity switch
                        {
                            DownloadValidity.Missing => $"Temporary cache file missing: {relativePathToTemporaryFile}",
                            DownloadValidity.LengthMismatch =>
                                $"Expected {currentFile.Size} bytes but received {temporaryFileInfo.Length}: {relativePathToTargetFile}",
                            DownloadValidity.ChecksumMismatch =>
                                $"Checksum mismatch, expected {currentFile.Checksum} but received {temporaryChecksum}: {relativePathToTemporaryFile}",
                            DownloadValidity.Valid => throw new UnreachableException(),
                            _ => throw new UnreachableException(),
                        }
                    );
                    ApplicationContext.CurrentContext.Logger.LogError(
                        Exception,
                        "Error detected while downloading streamed asset"
                    );
                    _failed = true;
                    Status = UpdateStatus.Error;
                    return false;
                }

                try
                {
                    if (targetFileInfo.Exists)
                    {
                        targetFileInfo.Delete();
                    }
                }
                catch (Exception exception)
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning(
                        exception,
                        "Failed to delete {RelativeTargetPath}",
                        relativePathToTargetFile
                    );

                    FileInfo oldTargetFileInfo = new($"{targetFileInfo.FullName}.old");
                    try
                    {
                        targetFileInfo.MoveTo(oldTargetFileInfo.FullName, true);
                    }
                    catch (Exception moveToException)
                    {
                        ApplicationContext.CurrentContext.Logger.LogWarning(
                            moveToException,
                            "Failed to move {RelativeSourcePath} to {RelativeTargetPath}.old",
                            relativePathToTargetFile,
                            relativePathToTargetFile
                        );
                    }
                }

                try
                {
                    temporaryFileInfo.MoveTo(targetFileInfo.FullName, true);

                    if (IsUpdaterFile(currentFile.Path))
                    {
                        _updaterContentChanged = true;
                    }

                    currentResult.State = DownloadState.Completed;
                    _downloadedBytes += currentFile.Size;

                    filesToStream.Remove(currentFile);
                }
                catch (Exception exception)
                {
                    ApplicationContext.CurrentContext.Logger.LogError(
                        exception,
                        "Failed to move {RelativeSourcePath} to {RelativeTargetPath}",
                        relativePathToTemporaryFile,
                        relativePathToTargetFile
                    );

                    try
                    {
                        using (var temporaryFileReadStream = temporaryFileInfo.OpenRead())
                        {
                            using var targetFileWriteStream = targetFileInfo.OpenWrite();
                            temporaryFileReadStream.CopyTo(targetFileWriteStream);
                        }

                        temporaryFileInfo.Delete();
                    }
                    catch (Exception copyException)
                    {
                        ApplicationContext.CurrentContext.Logger.LogError(
                            exception,
                            "Failed to copy data from {RelativeSourcePath} to {RelativeTargetPath}",
                            relativePathToTemporaryFile,
                            relativePathToTargetFile
                        );
                    }
                }
            }

            return false;
        }
        catch (EndOfStreamException endOfStreamException)
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                endOfStreamException,
                "Error download from stream: {RequestURI}",
                requestUri
            );

            foreach (var (file, result) in _downloadResults)
            {
                if (result.State != DownloadState.Failed)
                {
                    continue;
                }

                EnqueueLocked(file);
                result.State = DownloadState.Queued;
            }

            return false;
        }
        catch (Exception exception)
        {
            ApplicationContext.CurrentContext.Logger.LogError(
                exception,
                "Error download from stream"
            );
            return false;
        }
        finally
        {
            foreach (var file in filesToStream)
            {
                if (_downloadResults.TryGetValue(file, out var result) && result.State == DownloadState.Completed)
                {
                    continue;
                }

                EnqueueLocked(file);
            }
        }
    }

    private void EnqueueLocked(UpdateManifestFile updateManifestFile)
    {
        lock (_downloadQueue)
        {
            if (_downloadQueue.Contains(updateManifestFile))
            {
                return;
            }

            _downloadQueue.Enqueue(updateManifestFile);
        }
    }

    private bool TryDequeueLocked([NotNullWhen(true)] out UpdateManifestFile currentFile)
    {
        lock (_downloadQueue)
        {
            return _downloadQueue.TryDequeue(out currentFile) && currentFile != default;
        }
    }

    private bool DownloadUpdates(int workerThreadIndex, CancellationToken cancellationToken = default)
    {
        using IntersectHttpClient httpClient = new(_baseUrl, _tokenResponse);

        var buffer = new byte[MaxBuffer];
        // ReSharper disable line TooWideLocalVariableScope
        int bufferOffset, chunkSize, read;

        // ReSharper disable once InlineOutVariableDeclaration
        UpdateManifestFile currentFile;
        // ReSharper disable once InlineOutVariableDeclaration
        DownloadResult currentResult;

        while (!_failed && TryDequeueLocked(out currentFile))
        {
            bufferOffset = 0;

            lock (_downloadResults)
            {
                if (!_downloadResults.TryGetValue(currentFile, out currentResult))
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning(
                        $"Tried to download a file that was not registered, will ignore {nameof(UpdateManifest.OverrideBaseUrl)}: {currentFile.Path}"
                    );
                    currentResult = new DownloadResult { State = DownloadState.Active };
                    _downloadResults[currentFile] = currentResult;
                }

                switch (currentResult.State)
                {
                    case DownloadState.Completed:
                    case DownloadState.Active:
                    case DownloadState.Failed when currentResult.Count >= FailedRetryLimit:
                        continue;
                    case DownloadState.Queued:
                    default:
                        break;
                }

                ApplicationContext.CurrentContext.Logger.LogInformation($"[{workerThreadIndex}] Starting download for {currentResult.State} {currentFile.Path}");
                currentResult.State = DownloadState.Active;
            }

            try
            {
                var resolvedPath = ResolvePath(currentFile.Path, _updateRootInfo.FullName);
                FileInfo targetFileInfo = new(resolvedPath);
                FileInfo temporaryFileInfo = new($"{targetFileInfo}.tmp");

                var targetDirectoryInfo = targetFileInfo.Directory;
                if (targetDirectoryInfo is { Exists: false })
                {
                    targetDirectoryInfo.Create();
                }

                var effectiveBaseUrl = currentResult.OverrideBaseUrl ?? _baseUrl;
                var currentFileUrl = $"{effectiveBaseUrl}/{currentFile.Path}?token={Environment.TickCount}";

#if DIAGNOSTIC
                ApplicationContext.CurrentContext.Logger.LogTrace($"Starting download for {currentFile.Path}");
#endif
                using HttpRequestMessage requestMessage = new(HttpMethod.Get, currentFileUrl);
                using var responseMessage = httpClient.Send(requestMessage, cancellationToken);
#if DIAGNOSTIC
                ApplicationContext.CurrentContext.Logger.LogTrace($"Received response, beginning download of content for {(int)responseMessage.StatusCode} {currentFile.Path}");
#endif

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (responseMessage.StatusCode == HttpStatusCode.PartialContent)
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning($"Unsupported response from update server for \"{currentFile.Path}\" ({responseMessage.StatusCode})");
                    // TODO: Support 206 Partial Content
                    currentResult.State = DownloadState.Failed;
                    currentResult.Count = long.MaxValue;
                    continue;
                }

                if (responseMessage.StatusCode >= HttpStatusCode.InternalServerError)
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning($"Server error response from update server for \"{currentFile.Path}\" ({responseMessage.StatusCode})");
                    currentResult.State = DownloadState.Failed;
                    currentResult.Count += 1;
                    continue;
                }

                if (responseMessage.StatusCode >= HttpStatusCode.BadRequest)
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning($"Client error response from update server for \"{currentFile.Path}\" ({responseMessage.StatusCode})");
                    currentResult.State = DownloadState.Failed;
                    currentResult.Count = long.MaxValue;
                    continue;
                }

                if (currentFile.Size != responseMessage.Content.Headers.ContentLength)
                {
                    // This is fine for 206 Partial Content responses but not for any others
                    ApplicationContext.CurrentContext.Logger.LogWarning($"Update stream mismatch for \"{currentFile.Path}\" (expected {currentFile.Size} but received {responseMessage.Content.Headers.ContentLength})");
                    currentResult.State = DownloadState.Failed;
                    ++currentResult.Count;

                    if (currentResult.Count < FailedRetryLimit)
                    {
                        EnqueueLocked(currentFile);
                    }

                    continue;
                }

#if DIAGNOSTIC
                ApplicationContext.CurrentContext.Logger.LogTrace($"Opening response read stream for {currentFile.Path}");
#endif
                using var responseStream = responseMessage.Content.ReadAsStream(cancellationToken);
#if DIAGNOSTIC
                ApplicationContext.CurrentContext.Logger.LogTrace($"Opening file write stream for {currentFile.Path}");
#endif

                if (currentResult.State == DownloadState.Completed)
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning($"[{workerThreadIndex}] Two threads were downloading {currentResult.State} {currentFile.Path}");
                    continue;
                }

                ApplicationContext.CurrentContext.Logger.LogDebug($"[{workerThreadIndex}] Creating temporary file for {currentResult.State} {currentFile.Path}");
                using FileStream fileStream = temporaryFileInfo.OpenWrite();
#if DIAGNOSTIC
                ApplicationContext.CurrentContext.Logger.LogTrace($"Copying chunks of {currentFile.Path}");
#endif

                while (currentResult.Count < currentFile.Size && responseStream.Position < responseStream.Length)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    chunkSize = (int)Math.Min(
                        responseStream.Length - responseStream.Position,
                        Math.Min(
                            Math.Min(currentFile.Size - currentResult.Count, buffer.Length),
                            MaxBuffer - bufferOffset
                        )
                    );

#if DIAGNOSTIC
                    ApplicationContext.CurrentContext.Logger.LogTrace($"Reading chunk ({chunkSize} bytes) from {bufferOffset} {currentFile.Path}");
#endif
                    read = responseStream.Read(buffer, bufferOffset, chunkSize);

                    bufferOffset += read;
                    currentResult.Count += read;

#if DIAGNOSTIC
                    ApplicationContext.CurrentContext.Logger.LogTrace($"Read {read} (of {chunkSize}) bytes ({currentResult.Count}/{currentFile.Size}) of {currentFile.Path}");
#endif

                    if (bufferOffset < MaxBuffer)
                    {
                        continue;
                    }

                    fileStream.Write(buffer, 0, bufferOffset);
                    bufferOffset = 0;
                }

#if DIAGNOSTIC
                ApplicationContext.CurrentContext.Logger.LogTrace($"Writing final chunk of {currentFile.Path}");
#endif

                if (bufferOffset > 0)
                {
                    fileStream.Write(buffer, 0, bufferOffset);
                }

#if DIAGNOSTIC
                ApplicationContext.CurrentContext.Logger.LogTrace($"Flushing chunks to disk of {currentFile.Path}");
#endif

                fileStream.Close();

#if DIAGNOSTIC
                ApplicationContext.CurrentContext.Logger.LogTrace($"Finished download for {currentFile.Path}");
#endif

                var validity = ValidateTemporaryFile(currentFile, temporaryFileInfo, out var temporaryChecksum);

#if DIAGNOSTIC
                ApplicationContext.CurrentContext.Logger.LogTrace($"Checked validity of file (result: {validity}) {currentFile.Path}");
#endif

                var relativePathToTemporaryFile = Path.GetRelativePath(
                    _updateRootInfo.FullName,
                    temporaryFileInfo.FullName
                );

                var relativePathToTargetFile = Path.GetRelativePath(
                    _updateRootInfo.FullName,
                    targetFileInfo.FullName
                );

                if (validity != DownloadValidity.Valid)
                {
                    Exception = new InvalidDataException(
                        validity switch
                        {
                            DownloadValidity.Missing =>
                                $"Temporary cache file missing: {relativePathToTemporaryFile}",
                            DownloadValidity.LengthMismatch =>
                                $"Expected {currentFile.Size} bytes but received {temporaryFileInfo.Length}: {relativePathToTargetFile}",
                            DownloadValidity.ChecksumMismatch =>
                                $"Checksum mismatch, expected {currentFile.Checksum} but received {temporaryChecksum}: {relativePathToTemporaryFile}",
                            DownloadValidity.Valid => throw new UnreachableException(),
                            _ => throw new UnreachableException(),
                        }
                    );
                    ApplicationContext.CurrentContext.Logger.LogError(Exception, "Error downloading update");
                    _failed = true;
                    Status = UpdateStatus.Error;
                    return false;
                }

                if (IsSelf(currentFile.Path))
                {
                    ShouldRestart = true;

                    try
                    {
                        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            temporaryFileInfo.UnixFileMode |= UnixFileMode.UserExecute;
                        }
                    }
                    catch (Exception exception)
                    {
                        ApplicationContext.CurrentContext.Logger.LogWarning(exception, $"Failed to make {temporaryFileInfo.Name} executable");
                    }
                }

                try
                {
                    if (targetFileInfo.Exists)
                    {
                        targetFileInfo.Delete();
                    }
                }
                catch (Exception exception)
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning(exception, $"Failed to delete {relativePathToTargetFile}");
                }

                try
                {
                    temporaryFileInfo.MoveTo(targetFileInfo.FullName, true);

                    if (IsUpdaterFile(currentFile.Path))
                    {
                        _updaterContentChanged = true;
                    }

#if DIAGNOSTIC
                    ApplicationContext.CurrentContext.Logger.LogTraceApplicationContext.CurrentContext.Logger.LogTrace($"Completed for {currentFile.Path}");
#endif
                    currentResult.State = DownloadState.Completed;
                    _downloadedBytes += currentFile.Size;
                }
                catch (Exception exception)
                {
                    ApplicationContext.CurrentContext.Logger.LogError(
                        exception,
                        $"Failed to move {relativePathToTemporaryFile} to {relativePathToTargetFile}"
                    );

                    try
                    {
                        using (var temporaryFileReadStream = temporaryFileInfo.OpenRead())
                        {
                            using var targetFileWriteStream = targetFileInfo.OpenWrite();
                            temporaryFileReadStream.CopyTo(targetFileWriteStream);
                        }

                        temporaryFileInfo.Delete();
                    }
                    catch (Exception copyException)
                    {
                        ApplicationContext.CurrentContext.Logger.LogError(
                            copyException,
                            $"Failed to copy data from {relativePathToTemporaryFile} to {relativePathToTargetFile}"
                        );
                    }
                }
            }
            catch (Exception exception)
            {
                currentResult.State = DownloadState.Failed;
                var reattempt = ++currentResult.Count < FailedRetryLimit;
                var behaviorModifier = (reattempt ? string.Empty : "not ");
                var path = currentFile.Path;
                var message = $"Error while downloading file, will {behaviorModifier} reattempt: {path}";
                if (reattempt)
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning(exception, message);
                    EnqueueLocked(currentFile);
                }
                else
                {
                    ApplicationContext.CurrentContext.Logger.LogError(exception, message);
                }
            }

            Thread.Yield();
        }

        return true;
    }

    private static DownloadValidity ValidateTemporaryFile(
        UpdateManifestFile updateManifestFile,
        FileInfo temporaryFileInfo,
        out string? temporaryChecksum
    )
    {
        if (!temporaryFileInfo.Exists)
        {
            temporaryChecksum = default;
            return DownloadValidity.Missing;
        }

        if (temporaryFileInfo.Length != updateManifestFile.Size)
        {
            temporaryChecksum = default;
            return DownloadValidity.LengthMismatch;
        }

        temporaryChecksum = UpdateManifestFile.ComputeChecksum(temporaryFileInfo);
        return string.Equals(updateManifestFile.Checksum, temporaryChecksum, StringComparison.Ordinal)
            ? DownloadValidity.Valid
            : DownloadValidity.ChecksumMismatch;
    }

    private static void DeleteTemporaryFiles()
    {
        var oldFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.old", SearchOption.AllDirectories);
        var temporaryFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.tmp", SearchOption.AllDirectories);
        var filesToRemove = oldFiles.Concat(temporaryFiles).ToArray();
        foreach (var file in filesToRemove)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception exception)
            {
                ApplicationContext.CurrentContext.Logger.LogWarning(exception, $"Exception deleting old file: {file}");
            }
        }
    }

    private static bool IsUpdaterFile(string? path) =>
        !string.IsNullOrWhiteSpace(path) && path.StartsWith("resources/updater");

    private static bool IsSelf(string updatePath) => IsSelf(updatePath, out _);

    private static bool IsSelf(string updatePath, [NotNullWhen(true)] out string? pathToEntryAssembly)
    {
        pathToEntryAssembly = Assembly.GetEntryAssembly()?.Location;

#if !DEBUG
        if (string.IsNullOrWhiteSpace(pathToEntryAssembly))
        {
            pathToEntryAssembly = Environment.GetCommandLineArgs().FirstOrDefault();
        }
#endif

        if (string.IsNullOrWhiteSpace(pathToEntryAssembly))
        {
            return false;
        }

        var entryAssemblyFileName = Path.GetFileName(pathToEntryAssembly);

        // ReSharper disable once ConvertIfStatementToReturnStatement
        // The "self" update file path only counts if it's in the root
        var result = string.Equals(entryAssemblyFileName, updatePath, StringComparison.Ordinal);
        return result;
    }

    private static string ResolvePath(string updatePath, string basePath)
    {
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (!IsSelf(updatePath, out var pathToEntryAssembly))
        {
            return Path.GetFullPath(updatePath, basePath);
        }

        // TODO(FutureEditor): Reject self-updates
        return pathToEntryAssembly;
    }

    public static string GetHumanReadableFileSize(long size)
    {
        double len = size;
        int order = 0;
        while (len > 1024 && order < SizeSuffixes.Length - 1)
        {
            order++;
            len /= 1024;
        }
        return $"{len:0.##} {SizeSuffixes[order]}";
    }

    public bool CheckUpdaterContentLoaded()
    {
        if (!_updaterContentChanged)
        {
            return false;
        }

        _updaterContentChanged = false;
        return true;
    }

    public void Stop()
    {
        _stopping = true;
        if (_workerThreads != null)
        {
            foreach (var thread in _workerThreads)
            {
                try
                {
                    thread?.Interrupt();
                }
                catch (Exception exception)
                {
                    ApplicationContext.CurrentContext.Logger.LogWarning(exception, "Exception while interrupting download thread");
                }
            }
        }

        _stopping = true;
    }
}

internal enum DownloadValidity
{
    Valid,
    Missing,
    LengthMismatch,
    ChecksumMismatch,
}