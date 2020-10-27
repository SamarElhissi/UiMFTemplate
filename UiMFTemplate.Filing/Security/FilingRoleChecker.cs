namespace UiMFTemplate.Filing.Security
{
	using System.Collections.Generic;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;

	public class FilingRoleChecker : IUserRoleChecker
	{
		public IEnumerable<SystemRole> GetDynamicRoles(UserContextData userData)
		{
			if (userData != null)
			{
				yield return FilingRole.AuthenticatedUser;
			}
		}
	}
}