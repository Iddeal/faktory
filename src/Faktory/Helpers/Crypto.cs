using System;
using System.IO;
using System.Security.Cryptography;

namespace Faktory.Core.Helpers;

public static class Crypto
{
    public static string GetFileHash(string filePath)
    {
        using (var sha256 = SHA256.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                var hashBytes = sha256.ComputeHash(stream);
                return BitConverter
                    .ToString(hashBytes)
                    .Replace("-", "")
                    .ToUpperInvariant();
            }
        }
    }
}