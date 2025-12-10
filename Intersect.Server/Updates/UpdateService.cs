using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Intersect.Logging;

namespace Intersect.Server.Updates
{
    public class UpdateService
    {
        private readonly string _updatesPath;
        private readonly string _filesPath;
        private readonly string _manifestPath;
        private readonly string _baseUrl;

        public UpdateService(string basePath, string baseUrl)
        {
            _updatesPath = Path.Combine(basePath, "updates");
            _filesPath = Path.Combine(_updatesPath, "files");
            _manifestPath = Path.Combine(_updatesPath, "manifest.json");
            _baseUrl = baseUrl.TrimEnd('/');

            Directory.CreateDirectory(_updatesPath);
            Directory.CreateDirectory(_filesPath);
        }

        public async Task ProcessUpdateUpload(string version, Dictionary files)
        {
            try
            {
                Log.Info($"Processing update upload for version {version}");

                var manifest = new UpdateManifest
                {
                    Version = version,
                    Generated = DateTime.UtcNow
                };

                foreach (var file in files)
                {
                    var fileName = Path.GetFileName(file.Key);
                    var filePath = Path.Combine(_filesPath, fileName);

                    await File.WriteAllBytesAsync(filePath, file.Value);

                    var hash = CalculateMD5Hash(file.Value);

                    manifest.Files.Add(new UpdateFile
                    {
                        Name = fileName,
                        Size = file.Value.Length,
                        Hash = hash,
                        Url = $"{_baseUrl}/updates/files/{fileName}"
                    });

                    Log.Info($"Processed file: {fileName} ({file.Value.Length} bytes)");
                }

                var manifestJson = JsonConvert.SerializeObject(manifest, Formatting.Indented);
                await File.WriteAllTextAsync(_manifestPath, manifestJson);

                Log.Info($"Update manifest generated with {manifest.Files.Count} files");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error processing update upload: {ex.Message}");
                return false;
            }
        }

        public async Task GetManifest()
        {
            if (!File.Exists(_manifestPath))
            {
                return new UpdateManifest { Version = "0.0.0", Generated = DateTime.UtcNow };
            }

            var json = await File.ReadAllTextAsync(_manifestPath);
            return JsonConvert.DeserializeObject(json);
        }

        public byte[] GetUpdateFile(string fileName)
        {
            var filePath = Path.Combine(_filesPath, fileName);
            if (!File.Exists(filePath))
            {
                return null;
            }

            return File.ReadAllBytes(filePath);
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
}
