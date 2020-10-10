namespace UiMFTemplate.Web.Users
{
	using System;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.DataProtection;
	using Microsoft.AspNetCore.Identity;
	using Newtonsoft.Json;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Users;
	using UiMFTemplate.Users.Security;

	public class UserRoleChecker : IUserRoleChecker
	{
		private readonly IDataProtector protector;
		private readonly SignInManager<ApplicationUser> signInManager;
		private const string UserCookieName = "user-data";

		public UserRoleChecker(SignInManager<ApplicationUser> signInManager, IDataProtectionProvider protectionProvider)
		{
			this.signInManager = signInManager;
			this.protector = protectionProvider.CreateProtector("app-cookie");
		}
		public IEnumerable<SystemRole> GetDynamicRoles(UserContextData userData)
		{
			yield return userData != null
				? UserManagementRoles.AuthenticatedUser
				: UserManagementRoles.UnauthenticatedUser;

			this.signInManager.Context.Request.Cookies.TryGetValue(UserCookieName, out string cookieValue);
			UserSession userSession = null;

			if (cookieValue != null)
			{
				try
				{
					var json = this.protector.Unprotect(cookieValue);
					userSession = JsonConvert.DeserializeObject<UserSession>(json);
				}
				catch (Exception)
				{
					// Deserialization failed due to a corrupt cookie. Simply return null.
					userSession = null;
				}
			}
			if (userSession?.ImpersonatorUserId != null &&
				userSession.ImpersonatorUserId != userSession.CurrentUserId)
			{
				yield return UserManagementRoles.Impersonator;
			}
		}
	}
}
