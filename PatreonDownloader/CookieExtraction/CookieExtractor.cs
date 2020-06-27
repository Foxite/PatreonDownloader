namespace PatreonDownloader.CookieExtraction {
	public abstract class CookieExtractor {
		public abstract string Name { get; }

		public abstract string GetPatreonSessionToken();
	}
}
