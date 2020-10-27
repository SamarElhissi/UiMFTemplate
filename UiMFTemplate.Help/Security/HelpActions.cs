namespace UiMFTemplate.Help.Security
{
	using UiMFTemplate.Infrastructure.Security;

	public class HelpActions : ActionContainer
	{
		public static readonly SystemAction ViewHelpFiles = new SystemAction(nameof(ViewHelpFiles), HelpRoles.HelpReader);
	}
}