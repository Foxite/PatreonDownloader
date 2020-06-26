using Newtonsoft.Json;
using System;

namespace PatreonDownloader {
	[Serializable]
	public class PostPagePagination {
		[JsonProperty("cursors")]
		public PostPageCursors Cursors { get; set; }

		[JsonProperty("total")]
		public int Total { get; set; }
	}
}
