namespace UiMFTemplate.Web.Users.Commands
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using Microsoft.AspNetCore.Identity;
	using UiMetadataFramework.Basic.Response;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Users;
	using UiMFTemplate.Users.Commands;

	[MyForm(Id = "confirm-account", Label = "Confirming account...", PostOnLoad = true)]
	public class ConfirmAccount : AsyncForm<ConfirmAccount.Request, ReloadResponse>
	{
		private readonly ApplicationDbContext applicationDbContext;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly UserContext userContext;
		private readonly UserManager<ApplicationUser> userManager;

		public ConfirmAccount(UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			UserContext userContext,
			ApplicationDbContext applicationDbContext)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.userContext = userContext;
			this.applicationDbContext = applicationDbContext;
		}

		public override async Task<ReloadResponse> Handle(Request message, CancellationToken cancellationToken)
		{
			var user = this.userManager.Users.SingleOrDefault(t => t.Id == message.Id);

			if (user == null)
			{
				throw new BusinessException("Invalid parameters. Account could not be confirmed.");
			}

			var hasPassword = await this.userManager.HasPasswordAsync(user);
			if (user.EmailConfirmed && hasPassword)
			{
				return new ReloadResponse
				{
					Form = typeof(Login).GetFormId()
				};
			}

			if (!user.EmailConfirmed)
			{
				var result = await this.userManager.ConfirmEmailAsync(user, message.Token);
				var applicationUser = this.applicationDbContext.Users.SingleOrException(a => a.Id == user.Id);
				applicationUser.Activate();
				await this.applicationDbContext.SaveChangesAsync(cancellationToken);

				result.EnforceSuccess("Account was not confirmed.");
			}

			if (!this.userContext.IsAuthenticated)
			{
				await this.signInManager.SignInAsync(user, true);
			}

			return new ReloadResponse
			{
				Form = typeof(SetPassword).GetFormId(),
				InputFieldValues = new Dictionary<string, object>
				{
					{ nameof(SetPassword.Request.UserId), user.Id }
				}
			};
		}

		public class Request : IRequest<ReloadResponse>
		{
			[InputField(Hidden = true)]
			public int Id { get; set; }

			[InputField(Required = true, Hidden = true)]
			public string Token { get; set; }
		}
	}
}