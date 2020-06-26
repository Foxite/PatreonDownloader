using System;
using Newtonsoft.Json;

namespace PatreonDownloader {
	[Serializable]
	public class PostPageDataAttributes {
		/// <summary>
		/// Unknown.
		/// </summary>
		[JsonProperty("change_visibility_at")]
		public object ChangeVisibilityAt { get; set; }

		[JsonProperty("comment_count")]
		public int CommentCount { get; set; }

		[JsonProperty("like_count")]
		public int LikeCount { get; set; }

		[JsonProperty("content")]
		public string Content { get; set; }

		[JsonProperty("current_user_can_delete")]
		public bool CurrentUserCanDelete { get; set; }
		
		[JsonProperty("current_user_can_view")]
		public bool CurrentUserCanView { get; set; }

		[JsonProperty("current_user_has_liked")]
		public bool CurrentUserHasLiked { get; set; }

		/// <summary>
		/// Unknown.
		/// </summary>
		[JsonProperty("embed")]
		public object Embed { get; set; }

		[JsonProperty("image")]
		public PostPageDataAttributesImage Image { get; set; }

		[JsonProperty("is_paid")]
		public bool IsPaid { get; set; }

		[JsonProperty("min_cents_pledged_to_view")]
		public int? MinimumCentsPledgedToView { get; set; }

		[JsonProperty("patreon_url")]
		public string PatreonUrl { get; set; }

		[JsonProperty("pledge_url")]
		public string PledgeUrl { get; set; }

		[JsonProperty("post_file")]
		public PostPageDataAttributesPostFile PostFile { get; set; }

		[JsonProperty("post_metadata")]
		public PostPageDataAttributesMetadata PostMetadata { get; set; }

		[JsonProperty("post_type")]
		public string PostType { get; set; }

		[JsonProperty("published_at")]
		public DateTime PublishedAt { get; set; }

		[JsonProperty("teaser_text")]
		public string TeaserText { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("upgrade_url")]
		public string UpgradeUrl { get; set; }

		[JsonProperty("url")]
		public string PostUrl { get; set; }

		[JsonProperty("was_posted_by_campaign_owner")]
		public bool WasPostedByCampaignOwner { get; set; }
	}
}
