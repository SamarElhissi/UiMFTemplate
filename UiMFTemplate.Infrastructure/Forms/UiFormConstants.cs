namespace UiMFTemplate.Infrastructure.Forms
{
	using MediatR;
	using UiMFTemplate.Infrastructure.Forms.Outputs;

	public static class UiFormConstants
	{
		public const string SearchLabel = "<i class='fa fa-search'></i> Search";
		public const string CheckIcon = "<i style='color:#32c5d2;' class='fa fa-check-circle'></i>";
		public const string QuestionIcon = "<i style='color:#c49f47;' class='fa fa-question-circle'></i>";
		public const string TimesIcon = "<i style='color:#e7505a;' class='fa fa-times-circle'></i>";
		public const string EditSubmitLabel = "Save changes";
		public const string EditLabel = "<i class='fa fa-edit'></i>";
		public const string DeleteLabel = "<i class='fa fa-times'></i>";
		public const string EditIconLabel = "<i class='far fa-edit'></i>";
		public const string ImpersonationLabel = "<i class='far fa-eye'></i>";
		public const string StopImpersonationLabel = "<i class='far fa-eye-slash'></i>";
		public const string NotificationsLabel = "<i class='far fa-bell' title='Notifications'></i>";
		public const string SubTabsTemplate = "subtabs-template";
		public const string SubTabsClass = "subtabs";

        public static string ExcelTemplateUrl(string templateKey, int? formId) =>
            $"/file/downloadExcelTemplate?templateName={templateKey}&formId={formId}";

        public const string InputsVerticalMultipleColumn = "inputs-vertical-multiple-column";
		public const string InputsVerticalTwoColumn = "inputs-vertical-two-column";
		public const string InputsVerticalOneColumn = "inputs-vertical-one-column";
		public const string OutputsVerticalOneColumn = "outputs-vertical-one-column";
		public const string InputsHorizontalOneColumn = "inputs-horizontal-one-column";
		public const string OutputsHorizontalOneColumn = "outputs-horizontal-one-column";
		public const string InputsHorizontalMultipleColumn = "inputs-horizontal-one-column";
        public const string InputsHorizontalTwoColumn = "inputs-horizontal-two-column";
        public const string OutputsHorizontalMultipleColumn = "outputs-horizontal-one-column";
		public const string OutputsVerticalMultipleColumn = "outputs-vertical-multiple-column";
		public const string OutputsVerticalTwoColumn = "outputs-vertical-two-column";

    public const string TargetBlank = "_blank";
    public const string UploadButtonLabel = "Download";
    public const string Filters = "<i class='fa fa-filter'></i> Filter";
    public const string DangerIcon = "<i class='fa fa-times-circle icon-danger'></i>";
    public const string SuccessIcon = "<i class='fa fa-check-circle icon-success'></i>";
    public const string WarningIcon = "<i class='fa fa-exclamation-triangle icon-warning'></i>";
    public const string Currency = "(UGX)";
    public const string FooterRow = "row-footer";

    public const string CardLayout = "card-layout";
    public const string InputsVerticalFourColumn = "inputs-vertical-four-column";

    public static string CounterForTopMenu(int count) => count == 0 ? "" : $"<span class='count'>{count}</span>";
		public static string FileUrl(int fileId) => $"/file/download?id={fileId}";

		public static Link ExportToExcelLink<T>(string form, string field, IRequest<T> message)
		{
			var parameters = message.GetExcelParameters();
			var url = $"/api/form/{form}/{field}/exportToExcel?{parameters}";
			return new Link(url, "Export to excel")
			{
				CssClass = "btn btn-success"
			};
		}
	}
}
