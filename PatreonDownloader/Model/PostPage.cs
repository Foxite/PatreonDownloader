using Newtonsoft.Json;

namespace PatreonDownloader {
	public class PostPage {
		[JsonProperty("data")]
		public PostPageData[] Data { get; set; }

		[JsonProperty("included")]
		public PostPageIncluded[] Included { get; set; }

		[JsonProperty("links")]
		public PostPageLinks Links { get; set; }

		[JsonProperty("meta")]
		public PostPageMeta Meta { get; set; }
	}
}
