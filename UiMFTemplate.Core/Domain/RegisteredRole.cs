namespace UiMFTemplate.Core.Domain
{
	using System.Collections.Generic;

	public class RegisteredRole
	{
		public string ConcurrencyStamp { get; set; }
		public int Id { get; set; }
		public string Name { get; set; }
		public string NormalizedName { get; set; }
		public virtual ICollection<RegisteredUserRole> RegisteredUserRoles { get; set; }
	}
}