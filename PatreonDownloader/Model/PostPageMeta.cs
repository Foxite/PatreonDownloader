using Newtonsoft.Json;
using System;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageMeta {
		[JsonProperty("pagination")]
		public PostPagePagination Pagination { get; set; }
	}
}
