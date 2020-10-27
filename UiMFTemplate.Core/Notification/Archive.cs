namespace UiMFTemplate.Core.Notification
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using Microsoft.EntityFrameworkCore;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Basic.Response;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Notifications;

	[MyForm(Id = "archive-notification", Label = "Archive")]
	public class Archive : AsyncForm<Archive.Request, Archive.Response>
	{
		private readonly NotificationsDbContext notificationsDbContext;
		private readonly UserContext userContext;

		public Archive(NotificationsDbContext notificationsDbContext, UserContext userContext)
		{
			this.notificationsDbContext = notificationsDbContext;
			this.userContext = userContext;
		}

		public static FormLink Button(int id)
		{
			return new FormLink
			{
				Label = "Archive",
				Form = typeof(Archive).GetFormId(),
				InputFieldValues = new Dictionary<string, object>
				{
					{ nameof(Request.Id), id }
				}
			}.WithAction(FormLinkActions.Run).WithCustomUi("btn-sm btn-success");
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var ntf = await this.notificationsDbContext.Notifications.SingleOrDefaultAsync(t => t.Id == message.Id, cancellationToken);

			if (ntf == null || !ntf.AccessibleToUser(this.userContext))
			{
				throw new BusinessException("The notification not found, or you don't have permission");
			}

			ntf.Archive();

			await this.notificationsDbContext.SaveChangesAsync(cancellationToken);

			return new Response
			{
				Form = typeof(MyNotifications).GetFormId()
			};
		}

		public class Request : IRequest<Response>
		{
			public int Id { get; set; }
		}

		public class Response : ReloadResponse
		{
		}
	}
}