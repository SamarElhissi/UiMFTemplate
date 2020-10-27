namespace UiMFTemplate.Infrastructure.Forms.CustomProperties
{
	using UiMetadataFramework.Core.Binding;

	public class InitiallyHideFormAttribute : StringPropertyAttribute
	{
		public InitiallyHideFormAttribute(string headerText)
			: base("initiallyHideForm", headerText)
		{
		}
	}
}