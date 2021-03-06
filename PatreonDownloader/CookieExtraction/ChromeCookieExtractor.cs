﻿using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace PatreonDownloader.CookieExtraction {
	public class ChromeCookieExtractor : CookieExtractor {
		public override string Name => "Chrome";

		public override string GetPatreonSessionToken() {
			// TODO support for more than 1 profile
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "User Data", "Default", "Cookies");

			if (!File.Exists(dbPath)) {
				throw new CookieExtractorException("Chrome is not installed.");
			}

			using var connection = new SqliteConnection($"Data Source={dbPath}");

			connection.Open();
			SqliteCommand command = connection.CreateCommand();
			command.CommandText = "SELECT encrypted_value FROM cookies\n" +
				"WHERE host_key = '.patreon.com' AND name = 'session_id'\n" +
				"LIMIT 1"
				;
			using SqliteDataReader reader = command.ExecuteReader();
			if (reader.Read()) {
				byte[] encryptedData = (byte[]) reader.GetValue(0);
				string base64key = JObject.Parse(File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "User Data", "Local State")))["os_crypt"]["encrypted_key"].ToObject<string>();
				byte[] key = ProtectedData.Unprotect(Convert.FromBase64String(base64key).Skip(5).ToArray(), null, DataProtectionScope.LocalMachine);
				return DecryptWithKey(encryptedData, key, 3);
			}

			throw new CookieExtractorException("No suitable cookie was found in Chrome's storage.");
		}

		// https://stackoverflow.com/a/60611673
		private string DecryptWithKey(byte[] message, byte[] key, int nonSecretPayloadLength) {
			const int KEY_BIT_SIZE = 256;
			const int MAC_BIT_SIZE = 128;
			const int NONCE_BIT_SIZE = 96;

			if (key == null || key.Length != KEY_BIT_SIZE / 8)
				throw new ArgumentException($"Key needs to be {KEY_BIT_SIZE} bit!", "key");
			if (message == null || message.Length == 0)
				throw new ArgumentException("Message required!", "message");

			using var cipherStream = new MemoryStream(message);
			using var cipherReader = new BinaryReader(cipherStream);

			var nonSecretPayload = cipherReader.ReadBytes(nonSecretPayloadLength);
			var nonce = cipherReader.ReadBytes(NONCE_BIT_SIZE / 8);
			var cipher = new GcmBlockCipher(new AesEngine());
			var parameters = new AeadParameters(new KeyParameter(key), MAC_BIT_SIZE, nonce);
			cipher.Init(false, parameters);
			var cipherText = cipherReader.ReadBytes(message.Length);
			var plainText = new byte[cipher.GetOutputSize(cipherText.Length)];
			try {
				var len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
				cipher.DoFinal(plainText, len);
			} catch (InvalidCipherTextException) {
				return null;
			}
			return Encoding.Default.GetString(plainText);
		}
	}
}
