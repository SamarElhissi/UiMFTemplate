namespace UiMFTemplate.Core.Security.Magic
{
	using UiMFTemplate.Core.Domain;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;

	public class MagicPermissionManager : EntityPermissionManager<UserContext, MagicAction, MagicRole, Magic>
	{
		public MagicPermissionManager() : base(new MagicRoleChecker())
		{
		}
	}
}