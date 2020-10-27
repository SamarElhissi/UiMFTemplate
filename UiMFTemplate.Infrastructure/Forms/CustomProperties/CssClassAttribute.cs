namespace UiMFTemplate.Infrastructure.Forms.CustomProperties
{
	using UiMetadataFramework.Core.Binding;

	public class CssClassAttribute : StringPropertyAttribute
	{
		public CssClassAttribute(params string[] value)
			: base("cssClass", value.Join(" "))
		{
		}
	}
}