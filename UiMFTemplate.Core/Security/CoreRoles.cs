namespace UiMFTemplate.Core.Security
{
	using UiMFTemplate.Infrastructure.Security;

	public class CoreRoles : RoleContainer
	{
		public static readonly SystemRole AuthenticatedUser = new SystemRole(nameof(AuthenticatedUser), true);
		public static readonly SystemRole SysAdmin = new SystemRole(nameof(SysAdmin));
        public static readonly SystemRole Supervisor = new SystemRole(nameof(Supervisor));
    }
}
