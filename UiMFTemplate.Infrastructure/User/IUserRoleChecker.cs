namespace UiMFTemplate.Infrastructure.User
{
	using System.Collections.Generic;
	using UiMFTemplate.Infrastructure.Security;

	/// <summary>
	/// Represents a class which can retrieve roles for the current user.
	/// </summary>
	public interface IUserRoleChecker
	{
		/// <summary>
		/// Gets roles for the current user.
		/// </summary>
		/// <param name="userData">Instance of <see cref="UserContextData"/> or null if user not authenticated.</param>
		/// <returns>List of <see cref="SystemRole"/>.</returns>
		IEnumerable<SystemRole> GetDynamicRoles(UserContextData userData);
	}
}