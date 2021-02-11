namespace UiMFTemplate.Infrastructure
{
	using CPermissions;
	using UiMFTemplate.Infrastructure.User;

	public class PermissionException : PermissionException<UserContext>
	{
		public PermissionException(string action, UserContext userContext) : base(new UserAction(action), userContext)
		{
		}
	}
}
