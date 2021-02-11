namespace UiMFTemplate.Conversations.Commands
{
	using System;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using Microsoft.Extensions.Options;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Conversations.Domain;
	using UiMFTemplate.Conversations.Emails;
	using UiMFTemplate.Conversations.Forms.Outputs;
	using UiMFTemplate.Conversations.Notification;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.Infrastructure.Emails;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Notifications;
	using UiMFTemplate.Users;

	[MyForm(Id = "add-comment")]
	public class AddComment : MyAsyncForm<AddComment.Request, AddComment.Response>
	{
		private readonly IOptions<AppConfig> appConfig;
		private readonly ApplicationDbContext applicationDbContext;
		private readonly ConversationsDbContext<int> context;
		private readonly ConversationManagerCollection conversationManagerCollection;
		private readonly EmailTemplateRegister emailSender;
		private readonly NotificationsDbContext notificationsDbContext;
		private readonly UserContext userContext;

		public AddComment(ConversationsDbContext<int> context,
			UserContext userContext,
			ApplicationDbContext applicationDbContext,
			NotificationsDbContext notificationsDbContext,
			ConversationManagerCollection conversationManagerCollection,
			EmailTemplateRegister emailSender,
			IOptions<AppConfig> appConfig)
		{
			this.context = context;
			this.userContext = userContext;
			this.applicationDbContext = applicationDbContext;
			this.notificationsDbContext = notificationsDbContext;
			this.conversationManagerCollection = conversationManagerCollection;
			this.emailSender = emailSender;
			this.appConfig = appConfig;
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var key = ConversationKey.Parse(message.Key);
			var securityRule = this.conversationManagerCollection.GetInstance(key.EntityType);

			if (!securityRule.CanAddNewComments(key.EntityId))
			{
				throw new PermissionException($"Comment on {key.EntityType} {key.EntityId}", this.userContext);
			}

			var conversation = this.context.EnsureConversation(message.Key);
			var comment = new Comment<int>(conversation.Id, Convert.ToInt32(this.userContext.User.UserId), message.Text);
			this.context.Comments.Add(comment);
			await this.context.SaveChangesAsync(cancellationToken);

			await this.PublishNotificationAndEmails(securityRule, key, message.Text);

			return new Response
			{
				Comment = GetConversation.GetComments(comment, this.applicationDbContext, this.userContext)
			};
		}

		private async Task PublishNotificationAndEmails(IConversationManager securityRule, ConversationKey key, string comment)
		{
			var participants = await securityRule.GetParticipants(key.EntityId);
			var users = participants.Participants.SelectMany(t => t.Users);
			var userParticipants = users.DistinctBy(a => a.Id).ToList();

			var link = securityRule.Link(key.EntityId).GetUrl();

			foreach (var user in userParticipants)
			{
				if (user.Id.ToString() != this.userContext.User.UserId)
				{
					var model = new CommentAddedEmail.Model(this.appConfig.Value, comment, user.Username, user.Email, link);
					await this.emailSender.SendEmail(user.Email, model);

					this.notificationsDbContext.Add(
						new Notification(
							new EntityReference(NotificationRecipientType.UserId.Value, user.Id.ToString()),
							new EntityReference(key.EntityType, key.EntityId.ToString()),
							$"New comment posted for {key.EntityType.Split('.').Last()} #{key.EntityId}",
							null));
				}
			}

			await this.notificationsDbContext.SaveChangesAsync();
		}

		public class Request : IRequest<Response>
		{
			[InputField(Required = true)]
			public string Key { get; set; }

			[InputField(Required = true)]
			public string Text { get; set; }
		}

		public class Response : FormResponse<MyFormResponseMetadata>
		{
			[NotField]
			public Comment Comment { get; set; }
		}
	}
}
