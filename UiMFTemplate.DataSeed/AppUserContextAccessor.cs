namespace UiMFTemplate.DataSeed
{
	using System;
	using System.Linq;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Identity;
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Users;

	internal class AppUserContextAccessor : UserContextAccessor
    {
        private readonly CoreDbContext dbContext;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserSession userSession;

        public AppUserContextAccessor(
            UserRoleCheckerRegister roleCheckerRegister,
            SignInManager<ApplicationUser> signInManager,
            CoreDbContext dbContext,
            UserSession userSession) :
            base(roleCheckerRegister)
        {
            this.signInManager = signInManager;
            this.dbContext = dbContext;
            this.userSession = userSession;
        }

        protected override ClaimsPrincipal GetPrincipal()
        {
            if (this.userSession == null)
            {
                return null;
            }

            var claim = new Claim(ClaimTypes.NameIdentifier, this.userSession.CurrentUserId.ToString());

            var identity = new ClaimsIdentity();
            identity.AddClaim(claim);

            this.signInManager.UserManager.Users.Where(t => t.Id.ToString() == this.userSession.CurrentUserId)
                .SelectMany(t => t.Roles)
                .Select(t => t.Role.Name)
                .ToList()
                .ForEach(role => identity.AddClaim(new Claim(ClaimTypes.Role, role)));

            return new ClaimsPrincipal(identity);
        }

        protected override UserContextData GetUserContextData()
        {
            return this.userSession != null
                ? this.GetUserContextDataFromDatabase(Convert.ToInt32(this.userSession.CurrentUserId))
                : null;
        }

        private UserContextData GetUserContextDataFromDatabase(int userId)
        {
            var user = this.signInManager.UserManager.Users.SingleOrException(t => t.Id == userId);


            return new UserContextData(
                user.UserName,
                userId.ToString());
        }
    }
}