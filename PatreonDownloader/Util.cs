using System;
using System.IO;
using System.Text.RegularExpressions;

namespace PatreonDownloader {
	public static class Util {
		// https://stackoverflow.com/a/847251
		public static string SanitizeFilename(string name) {
			string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
			string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

			return Regex.Replace(name, invalidRegStr, "_");
		}

		public static int ConsoleChoiceMenu(string question, params string[] options) {
			Console.WriteLine(question);
			for (int i = 0; i < options.Length; i++) {
				Console.WriteLine($"[{i + 1}] {options[i]}");
			}

			int choice;
			while (!(int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= options.Length)) {
				Console.WriteLine("Enter the number of the option you want.");
			}

			return choice - 1;
		}
	}
}
