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
	using UiMFTemplate.Infrastructure.Forms.Inputs;
	using UiMFTemplate.Infrastructure.Forms.Outputs;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Users.Security;

	[MyForm(Id = "change-email", Label = "Change email")]
	[Secure(typeof(UserActions), nameof(UserActions.ManageMyAccount))]
	public class ChangeEmail : MyAsyncForm<ChangeEmail.Request, ChangeEmail.Response>
	{
		private readonly UserContext userContext;
		private readonly UserManager<ApplicationUser> userManager;

		public ChangeEmail(UserManager<ApplicationUser> userManager, UserContext userContext)
		{
			this.userManager = userManager;
			this.userContext = userContext;
		}

		public static FormLink Button()
		{
			return new FormLink
			{
				Label = "Change email",
				Form = typeof(ChangeEmail).GetFormId()
			};
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var user = await this.userManager.FindByNameAsync(this.userContext.User.UserName);

			var passwordIsValid = await this.userManager.CheckPasswordAsync(
				user,
				message.Password.Value);

			if (!passwordIsValid)
			{
				throw new BusinessException("Password wrong");
			}

			var result = await this.userManager.SetEmailAsync(
				user,
				message.NewEmail?.Value);

			result.EnforceSuccess("Change email process failed");

			return new Response
			{
				Result = Alert.Success("The email changed successfully")
			}.WithGrowlMessage("The email changed successfully", GrowlNotificationStyle.Success);
		}

		public class Response : FormResponse<MyFormResponseMetadata>
		{
			public Alert Result { get; set; }
		}

		public class Request : IRequest<Response>
		{
			[InputField(Required = true)]
			public Email NewEmail { get; set; }

			[InputField(Required = true)]
			public Password Password { get; set; }
		}
	}
}
