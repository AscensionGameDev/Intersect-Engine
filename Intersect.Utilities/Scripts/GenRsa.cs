using System.IO;
using System.Security.Cryptography;

using Intersect.Crypto.Formats;

namespace Intersect.Utilities.Scripts
{

    public class GenRsa : Script
    {

        public override Result Run(string[] environmentArgs, string[] commandArgs)
        {
            var bits = 2048;
            var filename = "intersect.bek";

            if (commandArgs?.Length > 0)
            {
                if (!int.TryParse(commandArgs[0], out bits))
                {
                    return new Result
                    {
                        Code = 1,
                        Message = "Failed to parse the bit size."
                    };
                }

                if (bits % 2 != 0)
                {
                    return new Result
                    {
                        Code = 101,
                        Message = "Bit size must be a multiple of 2."
                    };
                }

                if (bits < 1)
                {
                    return new Result
                    {
                        Code = 102,
                        Message = "Bit size must be non-zero."
                    };
                }
            }

            if (commandArgs?.Length > 1)
            {
                if (string.IsNullOrWhiteSpace(commandArgs[1]))
                {
                    return new Result
                    {
                        Code = 2,
                        Message = "Empty filename."
                    };
                }
            }

            using (var rsa = new RSACryptoServiceProvider(bits))
            {
                var rsaPrivateKey = new RsaKey(rsa.ExportParameters(true));
                using (var privateStream = new FileStream($"private-{filename}", FileMode.Create))
                {
                    rsaPrivateKey.Write(privateStream);
                }

                var rsaPublicKey = new RsaKey(rsa.ExportParameters(false));
                using (var publicStream = new FileStream($"public-{filename}", FileMode.Create))
                {
                    rsaPublicKey.Write(publicStream);
                }
            }

            return new Result();
        }

    }

}
