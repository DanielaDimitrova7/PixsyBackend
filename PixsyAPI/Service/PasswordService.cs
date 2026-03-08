using System.Security.Cryptography;
using System.Text;

namespace PixsyAPI.Service
{
	public class PasswordService
	{
		public string HashPassword(string password)
		{
			using var sha256 = SHA256.Create();
			var bytes = Encoding.UTF8.GetBytes(password);
			var hash = sha256.ComputeHash(bytes);
			return Convert.ToBase64String(hash);
		}

		public bool VerifyPassword(string enteredPassword, string storedHash)
		{
			var enteredHash = HashPassword(enteredPassword);
			return enteredHash == storedHash;
		}
	}
}
