namespace UiMFTemplate.Infrastructure.Forms.Outputs
{
	using UiMetadataFramework.Basic.Output;

	public class MyFormLink : FormLink
	{
		public string ConfirmationMessage { get; set; }
		public string CssClass { get; set; }
		public string Target { get; set; }
	}

	public class LinkStyle
	{
		public const string Primary = "btn-primary";
		public const string Default = "btn-default";
		public const string DefaultSmall = "btn-default btn-sm";
		public const string Danger = "btn-danger";
		public const string Success = "btn-success";
		public const string DangerSmall = "btn-danger btn-sm";
		public const string PrimarySmall = "btn-primary btn-sm";
		public const string WarningSmall = "btn-warning btn-sm";
		public const string Small = "btn-sm";
		public const string SuccessSmall = "btn-success btn-sm";
		public const string DangerIcon = DangerSmall + " btn-icon";
	}
}
