using System;
using System.Security.Cryptography;

namespace EventManagementSystem.BLL.Healpers
{
	internal static class PasswordHashHandler
	{
		private const int SaltSize = 16; // 128 bit
		private const int KeySize = 32;  // 256 bit
		private const int Iterations = 10000;

		public static string HashPassword(string password)
		{
			using (var rng = RandomNumberGenerator.Create())
			{
				byte[] salt = new byte[SaltSize];
				rng.GetBytes(salt);

				using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA512))
				{
					byte[] key = pbkdf2.GetBytes(KeySize);

					// Combine salt + key
					byte[] hashBytes = new byte[SaltSize + KeySize];
					Array.Copy(salt, 0, hashBytes, 0, SaltSize);
					Array.Copy(key, 0, hashBytes, SaltSize, KeySize);

					// Convert to base64 for storage
					return Convert.ToBase64String(hashBytes);
				}
			}
		}

		public static bool VerifyPassword(string password, string storedHash)
		{
			byte[] hashBytes = Convert.FromBase64String(storedHash);

			if (hashBytes.Length != SaltSize + KeySize)
				return false;

			byte[] salt = new byte[SaltSize];
			Array.Copy(hashBytes, 0, salt, 0, SaltSize);

			byte[] storedKey = new byte[KeySize];
			Array.Copy(hashBytes, SaltSize, storedKey, 0, KeySize);

			using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA512))
			{
				byte[] key = pbkdf2.GetBytes(KeySize);
				return FixedTimeEquals(key, storedKey);
			}
		}

		private static bool FixedTimeEquals(byte[] a, byte[] b)
		{
			if (a.Length != b.Length)
				return false;

			int diff = 0;
			for (int i = 0; i < a.Length; i++)
			{
				diff |= a[i] ^ b[i];
			}
			return diff == 0;
		}
	}
}
