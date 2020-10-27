namespace UiMFTemplate.Infrastructure.Forms.CustomProperties
{
	using UiMetadataFramework.Core.Binding;

	public class SortableByAttribute : StringPropertyAttribute
	{
		public SortableByAttribute(string value)
			: base("sortableBy", value)
		{
		}
	}
}