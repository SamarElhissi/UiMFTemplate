namespace UiMFTemplate.Core.Commands
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
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

	[MyForm(Id = "close-Magic", PostOnLoad = true)]
	[Secure(typeof(CoreActions), nameof(CoreActions.CloseMagic))]
	public class CloseMagic : MyAsyncForm<CloseMagic.Request, CloseMagic.Response>
	{
		private readonly IOptions<AppConfig> appConfig;
		private readonly CoreDbContext context;
		private readonly EmailTemplateRegister emailSender;
		private readonly NotificationsDbContext notificationsDbContext;

		public CloseMagic(CoreDbContext context,
			EmailTemplateRegister emailSender,
			NotificationsDbContext notificationsDbContext,
			IOptions<AppConfig> appConfig)
		{
			this.context = context;
			this.emailSender = emailSender;
			this.notificationsDbContext = notificationsDbContext;
			this.appConfig = appConfig;
		}

		public static FormLink Button(int userId)
		{
			return new FormLink
			{
				Form = typeof(CloseMagic).GetFormId(),
				Label = "Close",
				InputFieldValues = new Dictionary<string, object>
				{
					{ nameof(Request.Id), userId }
				}
			}.WithAction(FormLinkActions.Run);
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var Magic = await this.context.Magics
				.Include(a => a.CreatedByUser)
				.SingleNotDeletedOrExceptionAsync(t => t.Id == message.Id, cancellationToken: cancellationToken);

			Magic.Close();
			await this.context.SaveChangesAsync(cancellationToken);

			var user = Magic.CreatedByUser;
			var model = new MagicClosedEmail.Model(user.Id, "", user.Name, user.Email,
				MagicDetails.Button(Magic.Id).AsUrl(this.appConfig.Value));
			await this.emailSender.SendEmail(user.Email, model);

			this.notificationsDbContext.Add(
				new Notification(
					new EntityReference(NotificationRecipientType.UserId.Value, user.Id.ToString()),
					new EntityReference(MagicNotificationManager.Key, Magic.Id.ToString()),
					$"Magic #{Magic.Id} closed",
					null));

			await this.notificationsDbContext.SaveChangesAsync(cancellationToken);

			return new Response()
				.WithGrowlMessage($"Magic #{Magic.Id} was closed.", GrowlNotificationStyle.Success);
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
