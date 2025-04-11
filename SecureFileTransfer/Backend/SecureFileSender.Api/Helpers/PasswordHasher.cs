using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace SecureFileSender.Api.Helpers;

public static class PasswordHasher
{
	public static string HashPassword(string password)
	{
		byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
		return Convert.ToBase64String(KeyDerivation.Pbkdf2(
			password: password,
			salt: salt,
			prf: KeyDerivationPrf.HMACSHA256,
			iterationCount: 100_000,
			numBytesRequested: 256 / 8));
	}

	public static bool Verify(string enteredPassword, string storedHash)
	{
		// Simplified check for demo purposes — in production, store + compare with salt
		var rehashed = HashPassword(enteredPassword);
		return storedHash == rehashed;
	}
}
