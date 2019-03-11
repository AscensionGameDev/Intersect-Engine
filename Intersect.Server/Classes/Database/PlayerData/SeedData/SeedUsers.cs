using System;
using System.Security.Cryptography;
using System.Text;
using Intersect.Server.Database.PlayerData;
using Intersect.Server.Database.PlayerData.Security;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Intersect.Server.Classes.Database.PlayerData.SeedData
{
    public class SeedUsers : SeedData<User>
    {
        private string GenerateSalt([NotNull] RNGCryptoServiceProvider rng)
        {
            var buffer = new byte[32];
            rng.GetBytes(buffer);
            return BitConverter.ToString(buffer).Replace("-", "");
        }

        private string HashPassword([NotNull] string salt, [NotNull] string password)
        {
            using (var algorithm = new SHA256Managed())
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
                var salted = $@"{BitConverter.ToString(hash).Replace("-", "")}{salt}";
                var saltedHash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(salted));
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        public override void Seed([NotNull] EntityTypeBuilder<User> entityTypeBuilder)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                for (var n = 1; n <= 30; ++n)
                {
                    var salt = GenerateSalt(rng);

                    var userRights = UserRights.None;
                    if (n <= 10)
                    {
                        userRights = UserRights.Admin;
                    } else if (n <= 20)
                    {
                        userRights = UserRights.Moderation;
                    }

                    entityTypeBuilder.HasData(new User
                    {
                        Salt = salt,
                        Name = $@"test{n}",
                        Email = $@"test{n}@test.test",
                        Password = HashPassword(salt, "test"),
                        Power = userRights
                    });
                }
            }
        }
    }

    public static class RandomNumberGeneratorExtensions
    {
        public static int NextInt([NotNull] this RandomNumberGenerator rng)
        {
            var buffer = new byte[4];
            rng.GetBytes(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static uint NextUInt([NotNull] this RandomNumberGenerator rng)
        {
            var buffer = new byte[4];
            rng.GetBytes(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }
    }
}
