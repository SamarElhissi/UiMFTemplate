namespace UiMFTemplate.Core.Domain
{
	public class RegisteredUserRole
	{
		// ReSharper disable once UnusedAutoPropertyAccessor.Local
		public virtual RegisteredRole RegisteredRole { get; set; }
		public virtual RegisteredUser RegisteredUser { get; set; }
		public int RoleId { get; set; }
		public int UserId { get; set; }
	}
}
