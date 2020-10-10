namespace UiMFTemplate.Core.Security
{
	using UiMFTemplate.Infrastructure.Security;

	public class CoreActions : ActionContainer
	{
		public static readonly SystemAction ManageSystem = new SystemAction(nameof(ManageSystem), CoreRoles.AuthenticatedUser);
        public static readonly SystemAction ViewNotifications = new SystemAction(nameof(ViewNotifications), CoreRoles.SysAdmin);
    }
}
