namespace UiMFTemplate.Filing.Security
{
	using UiMFTemplate.Infrastructure.Security;

	public class FilingRole : RoleContainer
	{
		public static readonly SystemRole AuthenticatedUser = new SystemRole(nameof(AuthenticatedUser), true);
	}
}
