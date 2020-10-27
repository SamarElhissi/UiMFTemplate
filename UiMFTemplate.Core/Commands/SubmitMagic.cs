namespace UiMFTemplate.Core.Commands
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.Options;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.Core.Emails;
	using UiMFTemplate.Core.Extensions;
	using UiMFTemplate.Core.Notification;
	using UiMFTemplate.Core.NotificationManagers;
	using UiMFTemplate.Core.Security;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.Infrastructure.Emails;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.ClientFunctions;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Notifications;
	using UiMFTemplate.Users;

	[MyForm(Id = "submit-Magic", PostOnLoad = true)]
	[Secure(typeof(CoreActions), nameof(CoreActions.CreateMagic))]
	public class SubmitMagic : MyAsyncForm<SubmitMagic.Request, SubmitMagic.Response>
	{
		private readonly IOptions<AppConfig> appConfig;
		private readonly CoreDbContext context;
		private readonly EmailTemplateRegister emailSender;
		private readonly NotificationsDbContext notificationsDbContext;
		private readonly UserManager<ApplicationUser> userManager;

		public SubmitMagic(CoreDbContext context,
			EmailTemplateRegister emailSender,
			NotificationsDbContext notificationsDbContext,
			IOptions<AppConfig> appConfig,
			UserManager<ApplicationUser> userManager)
		{
			this.context = context;
			this.emailSender = emailSender;
			this.notificationsDbContext = notificationsDbContext;
			this.appConfig = appConfig;
			this.userManager = userManager;
		}

		public static FormLink Button(int userId)
		{
			return new FormLink
			{
				Form = typeof(SubmitMagic).GetFormId(),
				Label = "Submit",
				InputFieldValues = new Dictionary<string, object>
				{
					{ nameof(Request.Id), userId }
				}
			}.WithAction(FormLinkActions.Run);
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var Magic = await this.context.Magics
				.SingleNotDeletedOrExceptionAsync(t => t.Id == message.Id, cancellationToken: cancellationToken);

			Magic.Submit();
			await this.context.SaveChangesAsync(cancellationToken);

			var supervisors = await this.userManager.GetUsersInRoleAsync(CoreRoles.Supervisor.Name);
			foreach (var user in supervisors)
			{
				var model = new MagicSubmittedEmail.Model("", user.UserName, user.Email, MagicDetails.Button(Magic.Id).AsUrl(this.appConfig.Value));
				await this.emailSender.SendEmail(user.Email, model);

				this.notificationsDbContext.Add(
					new Notification(
						new EntityReference(NotificationRecipientType.UserId.Value, user.Id.ToString()),
						new EntityReference(MagicNotificationManager.Key, Magic.Id.ToString()),
						$"New Magic submitted",
						null));
			}

			await this.notificationsDbContext.SaveChangesAsync(cancellationToken);

			return new Response()
				.WithGrowlMessage($"Magic #{Magic.Id} was submitted.", GrowlNotificationStyle.Success);
		}

		public class Response : FormResponse<MyFormResponseMetadata>
		{
		}

		public class Request : IRequest<Response>
		{
			[InputField(Hidden = true)]
			public int Id { get; set; }
		}
	}
}