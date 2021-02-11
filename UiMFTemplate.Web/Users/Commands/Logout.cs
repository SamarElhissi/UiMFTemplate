namespace UiMFTemplate.Web.Users.Commands
{
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using Microsoft.AspNetCore.Identity;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Basic.Response;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Users;
	using UiMFTemplate.Users.Security;

	[MyForm(Id = "logout", PostOnLoad = true, Label = "Logout", Menu = UserMenus.Account, MenuOrderIndex = 100)]
	[Secure(typeof(UserActions), nameof(UserActions.Logout))]
	public class Logout : AsyncForm<Logout.Request, ReloadResponse>
	{
		private readonly SignInManager<ApplicationUser> signInManager;

		public Logout(SignInManager<ApplicationUser> signInManager)
		{
			this.signInManager = signInManager;
		}

		public static FormLink Button(string label = null)
		{
			return new FormLink
			{
				Form = typeof(Logout).GetFormId(),
				Label = label ?? "Logout"
			};
		}

		public override async Task<ReloadResponse> Handle(Request message, CancellationToken cancellationToken)
		{
			// TODO: implement removal of cookies in web project.
			this.signInManager.Context.Response.Cookies.Delete("app-data");
			this.signInManager.Context.Response.Cookies.Delete("user-data");
			this.signInManager.Context.Response.Cookies.Delete("session");
			await this.signInManager.SignOutAsync();

			return new ReloadResponse
			{
				Form = typeof(Login).GetFormId()
			};
		}

		public class Request : IRequest<ReloadResponse>
		{
		}
	}
}
