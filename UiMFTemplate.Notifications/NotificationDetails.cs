namespace UiMFTemplate.Notifications
{
	using System.Collections.Generic;
	using MediatR;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.Outputs;

	[Form(Id = "notification-details", PostOnLoad = true)]
	public class NotificationDetails : MyForm<NotificationDetails.Request, NotificationDetail>
	{
		private readonly NotificationManagerRegister notificationManagerRegister;

		public NotificationDetails(
			NotificationManagerRegister notificationManagerRegister)
		{
			this.notificationManagerRegister = notificationManagerRegister;
		}

		public static FormLink<NotificationDetails> Button(string cotextType, string contextId)
		{
			return new FormLink<NotificationDetails>
			{
				Form = typeof(NotificationDetails).GetFormId(),
				InputFieldValues = new Dictionary<string, object>
				{
					{ nameof(Request.ContextType), cotextType },
					{ nameof(Request.ContextId), contextId }
				}
			};
		}

		protected override NotificationDetail Handle(Request message)
		{
			var repository = this.notificationManagerRegister.GetInstance(message.ContextType);
			return repository.GetLink(message.ContextId);
		}

		public class Request : IRequest<NotificationDetail>
		{
			public string ContextId { get; set; }
			public string ContextType { get; set; }
		}
	}
}
