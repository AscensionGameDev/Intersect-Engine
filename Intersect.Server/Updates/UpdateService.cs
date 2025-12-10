using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Intersect.Logging;

namespace Intersect.Server.Core.Updates
{
    public class UpdateService
    {
        private readonly string _workingDirectory;
        private readonly string _updatesPath;
        private readonly string _filesPath;
        private readonly string _manifestPath;
        private readonly string _baseUrl;
        private readonly object _uploadLock = new object();

        public UpdateService(string workingDirectory, string baseUrl)
        {
            _workingDirectory = workingDirectory ?? Environment.CurrentDirectory;
            _updatesPath = Path.Combine(_workingDirectory, "updates");
            _filesPath = Path.Combine(_updatesPath, "files");
            _manifestPath = Path.Combine(_updatesPath, "manifest.json");
            _baseUrl = baseUrl?.TrimEnd('/') ?? "http://localhost:5400";

            EnsureDirectoriesExist();
        }

        private void EnsureDirectoriesExist()
        {
            try
            {
                if (!Directory.Exists(_updatesPath))
                {
                    Directory.CreateDirectory(_updatesPath);
                    Log.Info($"Created updates directory: {_updatesPath}");
                }

                if (!Directory.Exists(_filesPath))
                {
                    Directory.CreateDirectory(_filesPath);
                    Log.Info($"Created files directory: {_filesPath}");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error creating update directories: {ex.Message}");
                throw;
            }
        }

        public async Task ProcessUpdateUpload(string version, Dictionary files)
        {
            lock (_uploadLock)
            {
                try
                {
                    Log.Info($"Processing update upload for version {version} with {files.Count} files");

                    var manifest = new UpdateManifest
                    {
                        Version = version,
                        Generated = DateTime.UtcNow
                    };

                    BackupExistingFiles(version);

                    foreach (var file in files)
                    {
                        var fileName = SanitizeFileName(file.Key);
                        var filePath = Path.Combine(_filesPath, fileName);

                        File.WriteAllBytes(filePath, file.Value);

                        var hash = CalculateMD5Hash(file.Value);

                        manifest.Files.Add(new UpdateFile
                        {
                            Name = fileName,
                            Size = file.Value.Length,
                            Hash = hash,
                            Url = $"{_baseUrl}/updates/files/{fileName}"
                        });

                        Log.Debug($"Processed file: {fileName} ({file.Value.Length:N0} bytes, hash: {hash})");
                    }

                    var manifestJson = JsonConvert.SerializeObject(manifest, Formatting.Indented);
                    File.WriteAllText(_manifestPath, manifestJson);

                    Log.Info($"✓ Update manifest generated successfully with {manifest.Files.Count} files");
                    
                    return new UpdateUploadResult
                    {
                        Success = true,
                        Message = $"Successfully uploaded {files.Count} files",
                        Version = version,
                        FileCount = files.Count,
                        TotalSize = files.Sum(f => f.Value.Length)
                    };
                }
                catch (Exception ex)
                {
                    Log.Error($"✗ Error processing update upload: {ex}");
                    return new UpdateUploadResult
                    {
                        Success = false,
                        Message = $"Failed to process update: {ex.Message}"
                    };
                }
            }
        }

        public UpdateManifest GetManifest()
        {
            try
            {
                if (!File.Exists(_manifestPath))
                {
                    Log.Warn("No manifest.json found, returning empty manifest");
                    return new UpdateManifest 
                    { 
                        Version = "0.0.0", 
                        Generated = DateTime.UtcNow,
                        Files = new List()
                    };
                }

                var json = File.ReadAllText(_manifestPath);
                var manifest = JsonConvert.DeserializeObject(json);
                return manifest ?? new UpdateManifest { Version = "0.0.0", Generated = DateTime.UtcNow };
            }
            catch (Exception ex)
            {
                Log.Error($"Error reading manifest: {ex.Message}");
                return new UpdateManifest { Version = "0.0.0", Generated = DateTime.UtcNow };
            }
        }
 
        public byte[] GetUpdateFile(string fileName)
        {
            try
            {
                fileName = SanitizeFileName(fileName);
                var filePath = Path.Combine(_filesPath, fileName);

                if (!File.Exists(filePath))
                {
                    Log.Warn($"Update file not found: {fileName}");
                    return null;
                }

                return File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                Log.Error($"Error reading update file {fileName}: {ex.Message}");
                return null;
            }
        }

        private void BackupExistingFiles(string version)
        {
            try
            {
                if (!Directory.Exists(_filesPath) || Directory.GetFiles(_filesPath).Length == 0)
                {
                    return;
                }

                var backupDir = Path.Combine(_updatesPath, "backups", $"backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}");
                Directory.CreateDirectory(backupDir);

                foreach (var file in Directory.GetFiles(_filesPath))
                {
                    var fileName = Path.GetFileName(file);
                    File.Copy(file, Path.Combine(backupDir, fileName), true);
                }

                Log.Info($"Backed up existing files to: {backupDir}");
            }
            catch (Exception ex)
            {
                Log.Warn($"Failed to backup existing files: {ex.Message}");
            }
        }

        private string SanitizeFileName(string fileName)
        {
            return Path.GetFileName(fileName);
        }

        private string CalculateMD5Hash(byte[] data)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    public class UpdateUploadResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Version { get; set; }
        public int FileCount { get; set; }
        public long TotalSize { get; set; }
    }
}
