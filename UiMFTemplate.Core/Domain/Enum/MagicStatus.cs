namespace UiMFTemplate.Core.Domain.Enum
{
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.Attributes;

	public enum MagicStatus
	{
		Draft = 1,

		[HtmlString(Html = "Submitted " + UiFormConstants.SuccessIcon)]
		Submitted = 2,

		[HtmlString(Html = "Closed " + UiFormConstants.SuccessIcon)]
		Closed = 3
	}
}
