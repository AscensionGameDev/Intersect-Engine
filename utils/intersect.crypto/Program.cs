using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Intersect.Crypto
{
    class Program
    {
        static void Main(string[] args)
        {
            //new GenRsa().Generate();
            var rsa1Private = new RSACryptoServiceProvider(2048);
            var rsa1Public = new RSACryptoServiceProvider(512);
            rsa1Public.ImportParameters(rsa1Private.ExportParameters(false));

            var actual1 = Encoding.UTF8.GetBytes("dis is a string");
            var encrypted11 = rsa1Private.Encrypt(actual1, true);
            var encrypted12 = rsa1Private.Encrypt(actual1, false);

            try
            {
                var decrypted11 = rsa1Public.Decrypt(encrypted11, true);
                Console.WriteLine($"OEAP Padding Decryption: {Encoding.UTF8.GetString(decrypted11)}");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to decrypt using OEAP Padding");
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
            }

            try
            {
                var decrypted12 = rsa1Public.Decrypt(encrypted12, false);
                Console.WriteLine($"Non-OEAP Padding Decryption: {Encoding.UTF8.GetString(decrypted12)}");
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to decrypt not using OEAP Padding");
                Console.WriteLine(exception.Message);
                Console.WriteLine(exception.StackTrace);
            }

            Console.Read();
        }
    }
}
