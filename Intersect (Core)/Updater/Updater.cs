using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Intersect.Configuration;
using Intersect.Logging;

using Newtonsoft.Json;

namespace Intersect.Updater
{
    public class Updater
    {

        private Thread mUpdateThread;
        private Update mUpdate;
        private Update mCachedVersion;
        private Update mCurrentVersion;
        private string mCurrentVersionPath;
        private Thread[] mDownloadThreads;
        private readonly int mDownloadThreadCount = 1;
        private ConcurrentStack<UpdateFile> mDownloadQueue = new ConcurrentStack<UpdateFile>();
        private ConcurrentDictionary<UpdateFile, long> mFailedDownloads = new ConcurrentDictionary<UpdateFile, long>();
        private ConcurrentBag<UpdateFile> mCompletedDownloads = new ConcurrentBag<UpdateFile>();
        private ConcurrentDictionary<UpdateFile, long> mActiveDownloads = new ConcurrentDictionary<UpdateFile, long>();
        private long mDownloadedBytes;
        private bool mFailed;
        private bool mStopping;
        private bool mUpdaterContentLoaded;
        private bool mIsClient;
        private string mConfigUrl;
        private string mBaseUrl;

        public float Progress => ((float)BytesDownloaded / (float)SizeTotal) * 100f;

        public int FilesRemaining => mDownloadQueue.Count +
                                     mActiveDownloads.Count;

        public long SizeRemaining => SizeTotal -
                                     BytesDownloaded;

        public int FilesTotal => mDownloadQueue.Count +
                                 mActiveDownloads.Count +
                                 mCompletedDownloads.Count;

        public long BytesDownloaded => mDownloadedBytes +
                                       mActiveDownloads.Values.Sum();

        public long SizeTotal { get; private set; }

        public UpdateStatus Status { get; private set; } = UpdateStatus.Checking;

        public Exception Exception { get; private set; }

        public bool ReplacedSelf { get; private set; }



        public Updater(string updateUrl, string currentVersionPath, bool isClient, int maxDownloadThreads = 10)
        {
            if (string.IsNullOrWhiteSpace(updateUrl))
            {
                Status = UpdateStatus.None;
                return;
            }

            mDownloadThreadCount = maxDownloadThreads;
            mCurrentVersionPath = currentVersionPath;
            mIsClient = isClient;

            mUpdateThread = new Thread(RunUpdates);
            mUpdateThread.Start();
        }


        private async void RunUpdates()
        {
            DeleteOldFiles();

            //Download Update Config
            using (WebClient wc = new WebClient())
            {
                try
                {
                    mConfigUrl = ClientConfiguration.Instance.UpdateUrl.TrimEnd(new char[] { '/' });
                    mBaseUrl = ClientConfiguration.Instance.UpdateUrl.TrimEnd(new char[] { '/' });
                    var uri = new Uri(ClientConfiguration.Instance.UpdateUrl);

                    //Specifying update.json themselves or some other file that generates the config... base url needs to be the folder containing it
                    if (Path.HasExtension(uri.AbsolutePath))
                    {
                        mBaseUrl = uri.AbsoluteUri.Remove(uri.AbsoluteUri.Length - uri.Segments.Last().Length).TrimEnd(new char[] { '/' });
                    }
                    else
                    {
                        mConfigUrl = ClientConfiguration.Instance.UpdateUrl.TrimEnd(new char[] { '/' }) + "/update.json";
                    }

                    var jsonBytes = wc.DownloadData(mConfigUrl + "?token=" + Environment.TickCount);
                    var json = Encoding.UTF8.GetString(jsonBytes);
                    mUpdate = JsonConvert.DeserializeObject<Update>(json);

                    var downloadFirst = new List<UpdateFile>();

                    var updateRequired = true;

                    if (File.Exists(mCurrentVersionPath))
                    {
                        mCachedVersion = JsonConvert.DeserializeObject<Update>(
                            File.ReadAllText(mCurrentVersionPath)
                        );


                        updateRequired = false;
                        foreach (var file in mUpdate.Files.Where(f => f.ClientIgnore == false))
                        {
                            var checkFile = mCachedVersion.Files.FirstOrDefault(f => f.Path == file.Path);
                            if (checkFile == null || checkFile.Size != file.Size || checkFile.Hash != file.Hash)
                            {
                                updateRequired = true;
                            }
                            else
                            {
                                if (!File.Exists(file.Path) || !mUpdate.TrustCache)
                                {
                                    updateRequired = true;
                                }
                            }
                        }

                    }

                    //If we are doing a forced full check or if we don't have a current version file then we will start from scratch
                    if (updateRequired)
                    {
                        //Remove Deleted Files
                        if (mCachedVersion != null)
                        {
                            foreach (var file in mCachedVersion.Files)
                            {
                                if (!mUpdate.Files.Any(f => f.Path == file.Path))
                                {
                                    if (File.Exists(file.Path))
                                    {
                                        try
                                        {
                                            File.Delete(file.Path);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                        }

                        //Copy Over 
                        mCurrentVersion = new Update();
                        foreach (var file in mUpdate.Files)
                        {
                            if ((mIsClient && file.ClientIgnore || !mIsClient && file.EditorIgnore))
                            {
                                if (mCachedVersion != null)
                                {
                                    var ignoredFile = mCachedVersion.Files.FirstOrDefault(f => f.Path == file.Path);
                                    if (ignoredFile != null)
                                    {
                                        mCurrentVersion.Files.Add(ignoredFile);
                                    }
                                }

                                continue;
                            }

                            if (File.Exists(file.Path))
                            {
                                //If json we will still trust the cache, this might be wrong but given that the client is constantly updating json files we really can't expect the hash to always match
                                if (mCachedVersion != null && Path.GetExtension(file.Path) == ".json")
                                {
                                    var cacheCompare = mCachedVersion.Files.FirstOrDefault(f => f.Path == file.Path);
                                    if (cacheCompare != null)
                                    {
                                        if (cacheCompare.Size == file.Size &&
                                            cacheCompare.Hash == file.Hash)
                                        {
                                            mCurrentVersion.Files.Add(file);

                                            continue;
                                        }
                                    }
                                }

                                //Otherwise let's compare hashes and potentially add it to the update list
                                var md5Hash = "";
                                using (var md5 = MD5.Create())
                                {
                                    using (var fs = File.OpenRead(file.Path))
                                    {
                                        if (fs.Length != file.Size)
                                        {
                                            AddToUpdateList(file, downloadFirst);
                                        }
                                        else
                                        {
                                            using (var stream = new BufferedStream(fs, 1200000))
                                            {
                                                md5Hash = BitConverter.ToString(md5.ComputeHash(stream))
                                                    .Replace("-", "")
                                                    .ToLowerInvariant();
                                            }

                                            if (md5Hash != file.Hash)
                                            {
                                                AddToUpdateList(file, downloadFirst);
                                            }
                                            else
                                            {
                                                mCurrentVersion.Files.Add(file);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                AddToUpdateList(file, downloadFirst);
                            }
                        }
                    }

                    foreach (var file in downloadFirst)
                    {
                        mDownloadQueue.Push(file);
                    }

                    if (mDownloadQueue.Count == 0)
                    {
                        Status = UpdateStatus.None;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    //Failed to fetch update info or deserialize!
                    Status = UpdateStatus.Error;
                    Exception = new Exception("[Update Check Failed!] - " + ex.Message, ex);
                    return;
                }
            }

            //Got our update list!
            foreach (var file in mDownloadQueue)
                SizeTotal += file.Size;


            Status = UpdateStatus.Updating;

            var streamingSuccess = false;

            if (!string.IsNullOrWhiteSpace(mUpdate.StreamingUrl))
            {
                streamingSuccess = await StreamDownloads();
            }

            if (!streamingSuccess)
            {
                //Spawn Download Threads
                var threadCount = Math.Min(mDownloadThreadCount, FilesTotal);
                mDownloadThreads = new Thread[threadCount];

                for (int i = 0; i < threadCount; i++)
                {
                    mDownloadThreads[i] = new Thread(DownloadUpdates);
                    mDownloadThreads[i].Start();
                }
            }

            while (Updating())
            {
                Thread.Sleep(10);
            }

            //Success or failure we will save the current version info here
            foreach (var file in mCompletedDownloads)
                mCurrentVersion.Files.Add(file);


            File.WriteAllText(
                mCurrentVersionPath,
                JsonConvert.SerializeObject(
                    mCurrentVersion, Formatting.Indented,
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore }
                )
            );


            if (mStopping)
            {
                return;
            }

            if (!mFailed)
            {
                if (ReplacedSelf)
                {
                    Status = UpdateStatus.Restart;
                }
                else
                {
                    Status = UpdateStatus.Done;
                }
            }
            else
            {
                Status = UpdateStatus.Error;
            }

        }

        private async Task<bool> StreamDownloads()
        {
            var client = new HttpClient();

            var files = new List<string>();

            while (!mDownloadQueue.IsEmpty)
            {
                if (mDownloadQueue.TryPop(out UpdateFile file))
                {
                    files.Add(file.Path);
                    mActiveDownloads.TryAdd(file,0);
                }
            }
            var msg = new HttpRequestMessage(HttpMethod.Post, mUpdate.StreamingUrl + "?token=" + Environment.TickCount)
            {
                Content = new StringContent(JsonConvert.SerializeObject(files), Encoding.UTF8, "application/json"),
            };
            var response = await client.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead);

            using (var str = await response.Content.ReadAsStreamAsync())
            {
                try
                {
                    using (var br = new BinaryReader(str))
                    {
                        while (true)
                        {
                            var name = br.ReadString();

                            var file = mActiveDownloads.Keys.FirstOrDefault(f => f.Path == name);
                            var size = (int)br.ReadInt64();

                            var dataStream = new MemoryStream(size);
                            var downloaded = 0;
                            while (downloaded < size)
                            {
                                var chunk = 1024 * 1024;
                                if (downloaded + chunk > size)
                                {
                                    chunk = size - downloaded;
                                }
                                dataStream.Write(br.ReadBytes(chunk),0,chunk);
                                downloaded += chunk;
                                if (file != null)
                                {
                                    mActiveDownloads[file] = downloaded;
                                }
                            }

                            var data = dataStream.ToArray();
                            dataStream.Close();
                            dataStream.Dispose();

                            if (file != null)
                            {
                                try
                                {
                                    BeforeFileDownload(file);
                                    CheckFileData(file, data);
                                    BeforeReplaceFile(file);


                                    //Save New File
                                    File.WriteAllBytes(file.Path, data);

                                    lock (mUpdate)
                                    {
                                        mCompletedDownloads.Add(file);
                                        mActiveDownloads.TryRemove(file, out long val);
                                        mDownloadedBytes += file.Size;

                                        if (IsUpdaterFile(file.Path))
                                        {
                                            mUpdaterContentLoaded = true;
                                        }
                                    }


                                }
                                catch (EndOfStreamException eof)
                                {
                                    return mDownloadQueue.IsEmpty && mActiveDownloads.IsEmpty;
                                }
                                catch (Exception ex)
                                {
                                    lock (mUpdate)
                                    {

                                        mActiveDownloads.TryRemove(file, out long val);

                                        if (mFailedDownloads.ContainsKey(file))
                                        {
                                            mFailedDownloads[file]++;
                                        }
                                        else
                                        {
                                            mFailedDownloads.TryAdd(file, 1);
                                        }

                                        if (mFailedDownloads[file] > 2)
                                        {
                                            Exception = new Exception("[" + file.Path + "] - " + ex.Message, ex);
                                            mFailed = true;
                                        }
                                        else
                                        {
                                            mDownloadQueue.Push(file);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (EndOfStreamException eof)
                {
                    //Good to go?
                    //TODO Check if any files are missing, if so return false and let the basic downloader code try to fetch them.
                    return true;
                }
                catch (Exception ex)
                {
                    //Errored
                    Log.Error("Failed to download streamed files, failure occured on " );
                    return false;
                }
            }
        }

        private bool Updating()
        {
            lock (mUpdate)
            {
                return (mDownloadQueue.Count > 0 || mActiveDownloads.Count > 0) && !mFailed && !mStopping;
            }
        }

        private async void DownloadUpdates()
        {
            while (mDownloadQueue.Count > 0 && !mFailed)
            {
                UpdateFile file = null;
                var streamDl = false;
                lock (mUpdate)
                {
                    if (mDownloadQueue.TryPop(out file))
                    {
                        mActiveDownloads.TryAdd(file, 0);
                    }
                }

                if (file != null)
                {
                    //Download File
                    BeforeFileDownload(file);

                    try
                    {
                        //Use WebClient to Download File To Memory
                        var wc = new WebClient();
                        wc.DownloadProgressChanged += ((sender, args) => mActiveDownloads[file] = args.BytesReceived);
                        var rawUri = mBaseUrl +
                                     "/" +
                                     Uri.EscapeUriString(file.Path) +
                                     "?token=" +
                                     Environment.TickCount;

                        var fileUri = new Uri(rawUri);
                        var fileData = await wc.DownloadDataTaskAsync(fileUri);
                        wc.Dispose();

                        CheckFileData(file, fileData);

                        BeforeReplaceFile(file);

                        //Save New File
                        File.WriteAllBytes(file.Path, fileData);

                        lock (mUpdate)
                        {
                            mCompletedDownloads.Add(file);
                            mActiveDownloads.TryRemove(file, out long val);
                            mDownloadedBytes += file.Size;

                            if (IsUpdaterFile(file.Path))
                            {
                                mUpdaterContentLoaded = true;
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        lock (mUpdate)
                        {
                            mActiveDownloads.TryRemove(file, out long val);


                            if (mFailedDownloads.ContainsKey(file))
                            {
                                mFailedDownloads[file]++;
                            }
                            else
                            {
                                mFailedDownloads.TryAdd(file, 1);
                            }

                            if (mFailedDownloads[file] > 2)
                            {
                                Exception = new Exception("[" + file.Path + "] - " + ex.Message, ex);
                                mFailed = true;
                            }
                            else
                            {
                                mDownloadQueue.Push(file);
                            }
                        }
                    }

                }
                Thread.Sleep(10);
            }
        }

        private void BeforeFileDownload(UpdateFile file)
        {
            //Create any parent directories for this file
            var dir = Path.GetDirectoryName(file.Path);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        private void CheckFileData(UpdateFile file, byte[] fileData)
        {
            if (fileData.Length != file.Size)
            {
                throw new Exception("[File Length Mismatch - Got " + fileData.Length + " bytes, Expected " + file.Size + "]");
            }

            //Check MD5
            var md5Hash = "";
            using (var md5 = MD5.Create())
            {
                using (var stream = new MemoryStream(fileData))
                {
                    md5Hash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "").ToLowerInvariant();
                }
            }

            if (md5Hash != file.Hash)
            {
                throw new Exception("File Hash Mismatch");
            }
        }

        private void BeforeReplaceFile(UpdateFile file)
        {
            //Delete .old first if exists
            if (File.Exists(file.Path + ".old"))
            {
                try
                {
                    File.Delete(file.Path + ".old");
                }
                catch { }
            }

            //Delete Existing File
            if (File.Exists(file.Path))
            {
                try
                {
                    File.Delete(file.Path);
                }
                catch
                {
                    try
                    {
                        File.Move(file.Path, file.Path + ".old");
                    }
                    catch
                    {
                        throw new Exception("Failed to delete or move existing file!");
                    }
                }
            }

            if (file.Path == Path.GetFileName(Assembly.GetEntryAssembly().Location))
            {
                ReplacedSelf = true;
            }
        }

        private void AddToUpdateList(UpdateFile file, List<UpdateFile> downloadFirst)
        {
            if (IsUpdaterFile(file.Path.ToLower()))
            {
                downloadFirst.Add(file);
            }
            else
            {
                mDownloadQueue.Push(file);
            }
        }

        private void DeleteOldFiles()
        {
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.old", SearchOption.AllDirectories))
            {
                try
                {
                    File.Delete(file);
                }
                catch { }
            }
        }

        private bool IsUpdaterFile(string path)
        {
            switch (path)
            {
                case "resources/updater/background.png":
                    return true;
                case "resources/updater/font.xnb":
                    return true;
                case "resources/updater/fontsmall.xnb":
                    return true;
                case "resources/updater/progressbar.png":
                    return true;
                default:
                    return false;
            }
        }

        public string GetHumanReadableFileSize(long size)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = size;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return String.Format("{0:0.##} {1} Left", len, sizes[order]);
        }

        public bool CheckUpdaterContentLoaded()
        {
            if (mUpdaterContentLoaded)
            {
                mUpdaterContentLoaded = false;

                return true;
            }

            return false;
        }

        public void Stop()
        {
            if (mDownloadThreads != null)
            {
                foreach (var dlThread in mDownloadThreads)
                {
                    try
                    {
                        dlThread?.Abort();
                    }
                    catch
                    {
                    }
                }
            }

            mStopping = true;
        }
    }
}
