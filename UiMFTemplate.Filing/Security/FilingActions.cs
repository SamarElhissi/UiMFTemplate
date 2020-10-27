namespace UiMFTemplate.Filing.Security
{
	using UiMFTemplate.Infrastructure.Security;

	public class FilingActions : ActionContainer
	{
		public static readonly SystemAction ViewFiles = new SystemAction(nameof(ViewFiles), FilingRole.AuthenticatedUser);
	}
}