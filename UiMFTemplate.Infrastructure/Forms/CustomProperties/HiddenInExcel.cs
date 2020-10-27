namespace UiMFTemplate.Infrastructure.Forms.CustomProperties
{
	using System;
	using UiMetadataFramework.Core.Binding;

	public class HiddenInExcel : Attribute, ICustomPropertyAttribute
	{
		public object GetValue()
		{
			return true;
		}

		public string Name { get; set; } = "hiddenInExcel";
	}
}