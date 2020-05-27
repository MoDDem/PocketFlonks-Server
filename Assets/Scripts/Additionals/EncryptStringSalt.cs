using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

class EncryptStringSalt {
	public static string[] HashPassword(string input) {
		string salt = CreateSalt(10);
		string hashedPass = GenerateHash(input, salt);

		return new string[2] { salt, hashedPass };
	}

	public static bool ComparePasswordHash(string input, string hash, string salt) {
		string userHash = GenerateHash(input, salt);

		return userHash == hash;
	}

	public static string CreateSalt(int size) {
		var rand = new RNGCryptoServiceProvider();
		var buffer = new byte[size];
		rand.GetBytes(buffer);

		return Convert.ToBase64String(buffer);
	}

	private static string GenerateHash(string input, string salt) {
		byte[] bytes = Encoding.ASCII.GetBytes(input + salt);

		SHA256Managed hashString = new SHA256Managed();
		byte[] hash = hashString.ComputeHash(bytes);

		return ByteArrayToHexString(hash);
	}

	private static string ByteArrayToHexString(byte[] ba) {
		StringBuilder hex = new StringBuilder(ba.Length * 2);
		foreach(byte b in ba)
			hex.AppendFormat("{0:x2}", b);
		return hex.ToString();
	}
}