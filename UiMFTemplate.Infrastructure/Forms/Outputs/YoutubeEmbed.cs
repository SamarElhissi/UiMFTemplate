namespace UiMFTemplate.Infrastructure.Forms.Outputs
{
	using UiMetadataFramework.Core.Binding;

	[OutputFieldType("youtube-embed")]
	public class YoutubeEmbed
	{
		public YoutubeEmbed()
		{
		}

		public YoutubeEmbed(string html)
		{
			this.Value = html;
		}

		/// <summary>
		/// Gets or sets HTML string to render in the client.
		/// </summary>
		public string Value { get; set; }
	}
}