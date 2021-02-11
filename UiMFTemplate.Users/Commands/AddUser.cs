namespace UiMFTemplate.Users.Commands
{
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.Options;
	using UiMetadataFramework.Basic.Input.Typeahead;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.Inputs;
	using UiMFTemplate.Infrastructure.Messages;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Users.Pickers;
	using UiMFTemplate.Users.Security;

	[MyForm(Id = "add-user", Label = "Add new user")]
	[Secure(typeof(UserActions), nameof(UserActions.ManageUsers))]
	public class AddUser : MyAsyncForm<AddUser.Request, AddUser.Response>
	{
		private readonly AppConfig appConfig;
		private readonly IEmailSender emailSender;
		private readonly RoleManager<ApplicationRole> roleManager;
		private readonly UserManager<ApplicationUser> userManager;

		public AddUser(
			UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager,
			IEmailSender emailSender,
			IOptions<AppConfig> appConfig)
		{
			this.userManager = userManager;
			this.roleManager = roleManager;
			this.emailSender = emailSender;
			this.appConfig = appConfig.Value;
		}

		public static FormLink Button()
		{
			return new FormLink
			{
				Form = typeof(AddUser).GetFormId(),
				Label = "Add new user"
			};
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var applicationUser = new ApplicationUser
			{
				Email = message.Email?.Value,
				UserName = message.Name
			};

			var existUser = await this.userManager.FindByEmailAsync(applicationUser.Email);

			if (existUser != null)
			{
				throw new BusinessException("The email is used already, please try again.");
			}

			var identityResult = await this.userManager.CreateAsync(applicationUser);

			await applicationUser.SendConfirmationEmail(this.appConfig, this.emailSender, this.userManager);

			identityResult.EnforceSuccess("Add user process failed");

			if (message.Roles?.Items?.Count > 0)
			{
				await this.roleManager.EnsureRoles(message.Roles.Items);
				identityResult = await this.userManager.AddToRolesAsync(applicationUser, message.Roles.Items);
			}

			identityResult.EnforceSuccess("Add user process failed");

			return new Response
			{
				UserId = applicationUser.Id
			};
		}

		public class Request : IRequest<Response>
		{
			[InputField(OrderIndex = 10)]
			public Email Email { get; set; }

			[InputField(OrderIndex = 0)]
			public string Name { get; set; }

			[TypeaheadInputField(typeof(RoleTypeaheadInlineSource), OrderIndex = 20)]
			public MultiSelect<string> Roles { get; set; }
		}

		public class Response : MyFormResponse
		{
			[OutputField(Hidden = true)]
			public int UserId { get; set; }
		}
	}
}
