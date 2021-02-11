namespace UiMFTemplate.Core.Security
{
	using UiMFTemplate.Infrastructure.Security;

	public class CoreActions : ActionContainer
	{
		public static readonly SystemAction ManageSystem = new SystemAction(nameof(ManageSystem), CoreRoles.AuthenticatedUser);

		public static readonly SystemAction ViewNotifications =
			new SystemAction(nameof(ViewNotifications), CoreRoles.SysAdmin, CoreRoles.Supervisor, CoreRoles.User);

		public static readonly SystemAction CreateMagic = new SystemAction(nameof(CreateMagic), CoreRoles.User);
		public static readonly SystemAction ViewMagic = new SystemAction(nameof(ViewMagic), CoreRoles.User, CoreRoles.Supervisor);
		public static readonly SystemAction ViewAllMagic = new SystemAction(nameof(ViewAllMagic), CoreRoles.SysAdmin);
		public static readonly SystemAction CloseMagic = new SystemAction(nameof(CloseMagic), CoreRoles.Supervisor);
	}
}
