namespace UiMFTemplate.Infrastructure.User
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;

	/// <summary>
	/// Gives ability to retrieve information about current user and build corresponding
	/// <see cref="UserContext"/> object.
	/// </summary>
	public abstract class UserContextAccessor
	{
		private readonly UserRoleCheckerRegister roleCheckerRegister;

		protected UserContextAccessor(UserRoleCheckerRegister roleCheckerRegister)
		{
			this.roleCheckerRegister = roleCheckerRegister;
		}

		/// <summary>
		/// Retrieves <see cref="UserContext"/> for the current user.
		/// </summary>
		/// <returns><see cref="UserContext"/> instance.</returns>
		/// <remarks>This method uses <see cref="IUserRoleChecker.GetDynamicRoles"/> to populate
		/// <see cref="UserContext.Roles"/>. For this reason this method must not be called
		/// from <see cref="IUserRoleChecker.GetDynamicRoles"/>, because it will cause
		/// <see cref="StackOverflowException"/>.</remarks>
		public UserContext GetUserContext()
		{
			var userContextData = this.GetUserContextData();
			var systemRoles = new List<string>();
			if (userContextData != null)
			{
				systemRoles = this.GetUserRoles(Convert.ToInt32(userContextData.UserId)).ToList();
			}

			var dynamicRoles = this.roleCheckerRegister
				.GetDynamicRoles(userContextData)
				.Select(t => t.Name)
				.Distinct()
				.ToArray();

			var allRoles = dynamicRoles.Union(systemRoles).Distinct().ToArray();

			return new UserContext(userContextData, allRoles);
		}

		/// <summary>
		/// Gets principal for the current user.
		/// </summary>
		/// <returns>List of current user's roles.</returns>
		protected abstract ClaimsPrincipal GetPrincipal();

		/// <summary>
		/// Gets additional data about the current user.
		/// </summary>
		/// <returns><see cref="UserContextData"/> instance.</returns>
		protected abstract UserContextData GetUserContextData();

		protected abstract IList<string> GetUserRoles(int userId);
	}
}