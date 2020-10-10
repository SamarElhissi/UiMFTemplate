namespace UiMFTemplate.Infrastructure.Forms
{
	using UiMFTemplate.Infrastructure.Forms.Outputs;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;

	public class MyFormResponseMetadata : FormResponseMetadata
	{
		public MyFormResponseMetadata()
		{
		}

		public MyFormResponseMetadata(string handler) : base(handler)
		{
		}

		/// <summary>
		/// Gets or sets heading to show for this particular response instance.
		/// </summary>
		public string Title { get; set; }

		[OutputField(OrderIndex = -10)]
		public Alert Status { get; set; }

	}
}
