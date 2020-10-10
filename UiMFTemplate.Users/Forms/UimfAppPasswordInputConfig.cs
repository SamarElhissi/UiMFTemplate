namespace UiMFTemplate.Users.Forms
{
	using UiMFTemplate.Infrastructure.Forms.CustomProperties;

	public class UiMFTemplatePasswordInputConfig : PasswordInputConfigAttribute
	{
		public UiMFTemplatePasswordInputConfig(bool requireConfirmation = false)
		{
			this.RegexDescription =
                "The Password must be 8 characters  " +
				"at least one number, upper case character and lower case character.";

			this.Regex = "(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{8,}";

			this.RequireConfirmation = requireConfirmation;
		}
	}
}
