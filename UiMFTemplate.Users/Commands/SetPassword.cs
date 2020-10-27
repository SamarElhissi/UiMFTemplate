namespace UiMFTemplate.Users.Commands
{
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using Microsoft.AspNetCore.Identity;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Basic.Response;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Users.Forms;
	using UiMFTemplate.Users.Security;

	[MyForm(Id = "set-password", Label = "Reset password", SubmitButtonLabel = "Reset")]
	[Secure(typeof(UserActions), nameof(UserActions.SetPassword))]
	public class SetPassword : AsyncForm<SetPassword.Request, RedirectResponse>
	{
		private readonly UserManager<ApplicationUser> userManager;

		public SetPassword(UserManager<ApplicationUser> userManager)
		{
			this.userManager = userManager;
		}

		public override async Task<RedirectResponse> Handle(Request message, CancellationToken cancellationToken)
		{
			var user = this.userManager.Users.Single(t => t.Id == message.UserId);

			var result = await this.userManager.AddPasswordAsync(
				user,
				message.Password.Value);

			result.EnforceSuccess("Password reset failed.");

			return MyAccount.Button().AsRedirectResponse();
		}

		public class Request : IRequest<RedirectResponse>
		{
			[InputField(Required = true)]
			[MagicPasswordInputConfig(true)]
			public Password Password { get; set; }

			[InputField(Hidden = true)]
			public int UserId { get; set; }
		}
	}
}