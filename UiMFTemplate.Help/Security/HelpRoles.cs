namespace UiMFTemplate.Help.Security
{
	using UiMFTemplate.Infrastructure.Security;

	public class HelpRoles : RoleContainer
	{
		public static readonly SystemRole HelpReader = new SystemRole(nameof(HelpReader), true);
	}
}
