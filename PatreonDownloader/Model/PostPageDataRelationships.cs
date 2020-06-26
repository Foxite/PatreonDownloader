using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageDataRelationships {
		[JsonProperty("access_rules")]
		public DataArray AccessRules { get; set; }

		[JsonProperty("attachments")]
		public DataArray Attachments { get; set; }

		[JsonProperty("audio")]
		public DataArray Audio { get; set; }

		[JsonProperty("campaign")]
		public DataAndLinks Campaign { get; set; }

		[JsonProperty("images")]
		public DataArray Images { get; set; }

		[JsonProperty("media")]
		public DataArray Media { get; set; }

		[JsonProperty("poll")]
		public DataAndLinks Poll { get; set; }

		[JsonProperty("user")]
		public DataAndLinks User { get; set; }
	}
}
