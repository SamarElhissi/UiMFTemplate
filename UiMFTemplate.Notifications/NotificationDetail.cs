namespace UiMFTemplate.Notifications
{
	using UiMetadataFramework.Basic.Output;
	using UiMFTemplate.Infrastructure.Forms;
    using UiMetadataFramework.Core.Binding;

    public class NotificationDetail: MyFormResponse
	{
        [OutputField(Label = "")]
		public ActionList Actions { get; set; }
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets link to the related entity.
		/// </summary>
		public FormLink Link { get; set; }
	}
}
