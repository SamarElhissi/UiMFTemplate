namespace UiMFTemplate.Core.Security.Magic
{
	using System.Collections.Generic;
	using CPermissions;
	using UiMFTemplate.Core.Domain;
	using UiMFTemplate.Infrastructure.User;

	public class MagicRoleChecker : IRoleChecker<UserContext, MagicRole, Magic>
	{
		public IEnumerable<MagicRole> GetRoles(UserContext user, Magic context)
		{
			if (user.HasRole(CoreRoles.Supervisor) || user.HasRole(CoreRoles.SysAdmin))
			{
				yield return MagicRole.Manager;
			}

			if (context.CreatedByUserId.ToString() == user.User.UserId)
			{
				yield return MagicRole.Complainer;
			}
		}
	}
}