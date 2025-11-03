using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BusBookingSystem.Application
{
    public class BBSHashCode
    {
            // Hash password with salt
            public static string HashcodePassword(string password)
            {
                // Generate a random salt
                byte[] salt = new byte[16];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }

                // Derive a 256-bit subkey (PBKDF2 with SHA256, 100,000 iterations)
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);

                    // Combine salt + hash
                    byte[] hashBytes = new byte[48]; // 16 salt + 32 hash
                    Array.Copy(salt, 0, hashBytes, 0, 16);
                    Array.Copy(hash, 0, hashBytes, 16, 32);

                    // Convert to Base64 for storage
                    return Convert.ToBase64String(hashBytes);
                }
            }

            // Verify password
            public static bool VerifyPassword(string password, string storedHash)
            {
                byte[] hashBytes = Convert.FromBase64String(storedHash);

                // Extract salt
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);

                // Hash the entered password with the same salt
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);

                    // Compare hash results
                    for (int i = 0; i < 32; i++)
                    {
                        if (hashBytes[i + 16] != hash[i])
                            return false;
                    }
                }

                return true;
            }
        }
    }




