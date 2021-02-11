namespace UiMFTemplate.Infrastructure.Forms.Outputs
{
	using UiMetadataFramework.Core.Binding;

	[OutputFieldType("link")]
	public class Link
	{
		public Link(string url, string anchor)
		{
			this.Anchor = anchor;
			this.Url = url;
		}

		public string Anchor { get; }
		public string CssClass { get; set; }
		public string Target { get; set; } = "_self";
		public string Url { get; }
	}
}
