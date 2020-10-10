namespace UiMFTemplate.Infrastructure.Forms.CustomProperties
{
	using System;
	using UiMetadataFramework.Core.Binding;

	public class ReceiptListConfigAttribute : Attribute, ICustomPropertyAttribute
	{
		public ReceiptListConfigAttribute(string source)
        {
            this.Source = source;
        }

		public ReceiptListConfigAttribute()
        {
        }


		public string Source { get; private set; }

        public object GetValue()
		{
			return new
			{
				this.Source
			};
		}

		public string Name { get; set; } = "receiptListConfig";
	}
}
