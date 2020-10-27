namespace UiMFTemplate.Help.Security
{
	using System.Collections.Generic;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;

	public class HelpRoleChecker : IUserRoleChecker
	{
		public IEnumerable<SystemRole> GetDynamicRoles(UserContextData userData)
		{
			if (userData != null)
			{
				yield return HelpRoles.HelpReader;
			}
		}
	}
}