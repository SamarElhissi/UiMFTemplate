namespace UiMFTemplate.Users
{
	using System;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Identity;

	public class ApplicationUser : IdentityUser<int>
	{
		public bool Active => this.DateActivated.HasValue;

		/// <summary>
		/// Navigation property for the claims this user possesses.
		/// </summary>
		public virtual ICollection<ApplicationUserClaim> Claims { get; } = new List<ApplicationUserClaim>();

		public DateTime? DateActivated { get; set; }

		public bool HasLoggedIn => this.EmailConfirmed || this.PasswordHash != null;

		/// <summary>
		/// Navigation property for this users login accounts.
		/// </summary>
		public virtual ICollection<ApplicationUserLogin> Logins { get; } = new List<ApplicationUserLogin>();

		/// <summary>
		/// Navigation property for the roles this user belongs to.
		/// </summary>
		public virtual ICollection<ApplicationUserRole> Roles { get; } = new List<ApplicationUserRole>();

		public void Activate()
		{
			this.DateActivated = DateTime.UtcNow;
		}

		public void Deactivate()
		{
			this.DateActivated = null;
		}
	}
}