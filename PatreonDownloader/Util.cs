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
	}
}
