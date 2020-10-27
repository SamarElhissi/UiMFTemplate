namespace UiMFTemplate.Users.Commands
{
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using Microsoft.AspNetCore.Identity;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.ClientFunctions;
	using UiMFTemplate.Infrastructure.Forms.Outputs;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Users.Forms;
	using UiMFTemplate.Users.Security;

	[MyForm(Id = "change-password", Label = "Change password")]
	[Secure(typeof(UserActions), nameof(UserActions.ManageMyAccount))]
	public class ChangePassword : MyAsyncForm<ChangePassword.Request, ChangePassword.Response>
	{
		private readonly UserContext userContext;
		private readonly UserManager<ApplicationUser> userManager;

		public ChangePassword(UserManager<ApplicationUser> userManager, UserContext userContext)
		{
			this.userManager = userManager;
			this.userContext = userContext;
		}

		public static FormLink Button()
		{
			return new FormLink
			{
				Label = "Change password",
				Form = typeof(ChangePassword).GetFormId()
			};
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var user = await this.userManager.FindByNameAsync(this.userContext.User.UserName);

			var result = await this.userManager.ChangePasswordAsync(
				user,
				message.OldPassword.Value,
				message.NewPassword.Value);

			result.EnforceSuccess("Change password failed");

			return new Response
			{
				Result = Alert.Success("Password changed successfully")
			}.WithGrowlMessage("Password changed successfully", GrowlNotificationStyle.Success);
		}

		public class Response : FormResponse<MyFormResponseMetadata>
		{
			public Alert Result { get; set; }
		}

		public class Request : IRequest<Response>
		{
			[InputField(Required = true, OrderIndex = 10)]
			[MagicPasswordInputConfig(true)]
			public Password NewPassword { get; set; }

			[InputField(Required = true, OrderIndex = 0)]
			public Password OldPassword { get; set; }
		}
	}
}