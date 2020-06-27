using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace PatreonDownloader.CookieExtraction {
	public class FirefoxCookieExtractor : CookieExtractor {
		public override string Name => "Firefox";

		public override string GetPatreonSessionToken() {
			string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Mozilla", "Firefox", "Profiles");

			if (!Directory.Exists(dbPath)) {
				throw new CookieExtractorException("Firefox is not installed.");
			}

			var profiles = Directory.GetDirectories(dbPath);

			dbPath = Path.Combine(profiles[Util.ConsoleChoiceMenu("Which profile do you want to use?", profiles)], "Cookies.sqlite");

			using var connection = new SqliteConnection($"Data Source={dbPath}");

			connection.Open();
			SqliteCommand command = connection.CreateCommand();
			command.CommandText = "SELECT value FROM moz_cookies\n" +
				"WHERE host = '.patreon.com' AND name = 'session_id'\n" +
				"LIMIT 1"
				;
			using SqliteDataReader reader = command.ExecuteReader();
			if (reader.Read()) {
				return reader.GetString(0);
			}

			throw new CookieExtractorException("No suitable cookie was found in Firefox' storage.");
		}
	}
}
