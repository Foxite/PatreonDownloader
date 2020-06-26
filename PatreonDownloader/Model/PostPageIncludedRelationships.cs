using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageIncludedRelationships {
		/// <summary>
		/// Only if <see cref="PostPageIncluded.Type"/> == <see cref="PostPageIncluded.IncludedType.User"/> or <see cref="PostPageIncluded.IncludedType.Reward"/>.
		/// </summary>
		[JsonProperty("campaign")]
		public DataAndLinks Campaign { get; set; }

		/// <summary>
		/// Only if <see cref="PostPageIncluded.Type"/> == <see cref="PostPageIncluded.IncludedType.Campaign"/>
		/// </summary>
		[JsonProperty("creator")]
		public DataAndLinks Creator { get; set; }

		/// <summary>
		/// Only if <see cref="PostPageIncluded.Type"/> == <see cref="PostPageIncluded.IncludedType.Poll"/>
		/// </summary>
		[JsonProperty("choices")]
		public DataArray Choices { get; set; }
	}
}
