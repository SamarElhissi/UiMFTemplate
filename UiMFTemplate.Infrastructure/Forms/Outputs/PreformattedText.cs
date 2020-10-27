// ReSharper disable UnusedMember.Global

namespace UiMFTemplate.Infrastructure.Forms.Outputs
{
	using UiMetadataFramework.Core.Binding;

	[OutputFieldType("preformatted-text")]
	public class PreformattedText
	{
		public PreformattedText()
		{
		}

		public PreformattedText(string text)
		{
			this.Value = text;
		}

		public string Value { get; set; }
	}
}