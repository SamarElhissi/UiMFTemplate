namespace UiMFTemplate.Infrastructure.Forms
{
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Infrastructure.Forms.Outputs;

	public class MyFormResponseMetadata : FormResponseMetadata
	{
		public MyFormResponseMetadata()
		{
		}

		public MyFormResponseMetadata(string handler) : base(handler)
		{
		}

		[OutputField(OrderIndex = -10)]
		public Alert Status { get; set; }

		/// <summary>
		/// Gets or sets heading to show for this particular response instance.
		/// </summary>
		public string Title { get; set; }
	}
}
