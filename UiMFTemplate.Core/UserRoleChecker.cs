namespace UiMFTemplate.Core
{
	using System.Collections.Generic;
	using UiMFTemplate.Core.Security;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;

	public class UserRoleChecker : IUserRoleChecker
	{
		public IEnumerable<SystemRole> GetDynamicRoles(UserContextData userData)
		{
			if (userData != null)
			{
				yield return CoreRoles.AuthenticatedUser;
			}
		}
	}
}