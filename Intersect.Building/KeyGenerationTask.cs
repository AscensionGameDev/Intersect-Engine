using System;
using System.IO;
using System.Security.Cryptography;

using Intersect.Building.Properties;
using Intersect.Crypto;
using Intersect.Crypto.Formats;

using JetBrains.Annotations;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Intersect.Building
{
    /// <summary>
    /// Task for generating network security keys automatically at build time.
    /// </summary>
    [UsedImplicitly]
    public class KeyGenerationTask : AppDomainIsolatedTask
    {
        /// <summary>
        /// Generate new keys each time the build is run.
        ///
        /// If false, only generate keys when they do not
        /// exist in <see cref="OutputDirectory"/>.
        /// </summary>
        public bool GenerateEachBuild { get; set; }

        /// <summary>
        /// Key size in bits, and should be a power of 2.
        ///
        /// Recommended to not lower this below 2048.
        /// </summary>
        public int KeySize { get; set; } = 4096;

        /// <summary>
        /// Directory path to output the generated keys to.
        /// </summary>
        [CanBeNull]
        [Required]
        public string OutputDirectory { get; set; }

        /// <inheritdoc />
        public override bool Execute()
        {
            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                Log?.LogError(KeyGenerationTaskStrings.OutputDirectoryInvalid);

                return false;
            }

            if (!Directory.Exists(OutputDirectory))
            {
                try
                {
                    Directory.CreateDirectory(OutputDirectory);
                }
                catch (Exception exception)
                {
                    Log?.LogError(KeyGenerationTaskStrings.ErrorCreatingOutputDirectory, OutputDirectory);
                    Log?.LogErrorFromException(exception);
                }
            }

            var power = Math.Log(KeySize, 0);
            if (KeySize % 2 != 0 || power < 10 || power - Math.Floor(power) > 0.001)
            {
                Log?.LogError(KeyGenerationTaskStrings.KeySizeInvalid, KeySize);

                return false;
            }

            var pathPrivateKey = Path.Combine(OutputDirectory, "network.handshake.bkey");
            var pathPublicKey = Path.Combine(OutputDirectory, "network.handshake.bkey.pub");

            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                var writePrivateKey = true;
                if (!GenerateEachBuild)
                {
                    if (File.Exists(pathPrivateKey))
                    {
                        if (File.Exists(pathPublicKey))
                        {
                            return true;
                        }

                        Log?.LogWarning(KeyGenerationTaskStrings.PublicKeyMissing);
                    }

                    try
                    {
                        using (var stream = File.OpenRead(pathPrivateKey))
                        {
                            var privateKey = new RsaKey();
                            privateKey.Read(stream);

                            if (privateKey.IsPublic || privateKey.Format != KeyFormat.Rsa)
                            {
                                Log?.LogWarning(KeyGenerationTaskStrings.PrivateKeyInvalid);
                            }
                            else
                            {
                                rsa.ImportParameters(privateKey.Parameters);
                                writePrivateKey = false;
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Log?.LogWarning(KeyGenerationTaskStrings.ErrorReadingPrivateKey);
                        Log?.LogWarningFromException(exception);
                    }
                }

                if (writePrivateKey)
                {
                    var privateKey = new RsaKey(rsa.ExportParameters(true));
                    try
                    {
                        using (var stream = File.OpenWrite(pathPrivateKey))
                        {
                            privateKey.Write(stream);
                        }
                    }
                    catch (Exception exception)
                    {
                        Log?.LogError(KeyGenerationTaskStrings.ErrorWritingPrivateKey);
                        Log?.LogErrorFromException(exception);

                        return false;
                    }
                }

                var publicKey = new RsaKey(rsa.ExportParameters(false));
                try
                {
                    using (var stream = File.OpenWrite(pathPublicKey))
                    {
                        publicKey.Write(stream);
                    }
                }
                catch (Exception exception)
                {
                    Log?.LogError(KeyGenerationTaskStrings.ErrorWritingPublicKey);
                    Log?.LogErrorFromException(exception);

                    return false;
                }

                return true;
            }
        }
    }
}
