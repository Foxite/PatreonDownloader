using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageDataAttributesMetadata {
		[JsonProperty("image_order")]
		public int[] ImageOrder { get; set; }
	}
}
