// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace UiMFTemplate.Core.Domain
{
	using System.Collections.Generic;

	/// <summary>
	/// Represents user registered in the system.
	/// </summary>
	public class RegisteredUser
	{
		private RegisteredUser()
		{
			// This constructor is private, because we are not supposed to create new users
			// from this library. All users are created by *UiMFTemplate.Users*. This assembly
			// can only read existing data.
		}

		public string Email { get; private set; }
		public int Id { get; private set; }
		public string Name { get; private set; }
		public virtual ICollection<RegisteredUserRole> RegisteredUserRoles { get; set; }

		public void Edit(string email)
		{
			this.Email = email;
			this.Name = email;
		}

		internal static RegisteredUser Create(int userId, string name)
		{
			return new RegisteredUser
			{
				Id = userId,
				Name = name
			};
		}
	}
}
