namespace UiMFTemplate.Conversations.Commands
{
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Conversations.Domain;
	using UiMFTemplate.Conversations.Forms.Outputs;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Users;

	[MyForm(Id = "conversations", PostOnLoad = true)]
	public class GetConversation : MyAsyncForm<GetConversation.Request, GetConversation.Response>
	{
		private readonly ApplicationDbContext applicationContext;
		private readonly ConversationsDbContext<int> context;
		private readonly ConversationManagerCollection conversationManagerCollection;
		private readonly UserContext userContext;

		public GetConversation(ConversationsDbContext<int> context,
			ConversationManagerCollection conversationManagerCollection,
			ApplicationDbContext applicationContext,
			UserContext userContext)
		{
			this.context = context;
			this.conversationManagerCollection = conversationManagerCollection;
			this.applicationContext = applicationContext;
			this.userContext = userContext;
		}

		public static Comment GetComments(Comment<int> comment, ApplicationDbContext applicationDbContext, UserContext userContext)
		{
			var author = applicationDbContext.Users.Find(comment.AuthorId);

			return new Comment
			{
				Author = author.UserName,
				Children = comment.Children.Select(t => GetComments(t, applicationDbContext, userContext)).ToList(),
				ContextId = comment.Id,
				ParentId = comment.ParentId,
				PostedOn = comment.PostedOn,
				Text = comment.Text,
				Id = comment.Id,
				CanDelete = comment.AuthorId.ToString() == userContext.User.UserId
			};
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var key = ConversationKey.Parse(message.Key);
			var securityRule = this.conversationManagerCollection.GetInstance(key.EntityType);
			var conversation = this.context.EnsureConversation(message.Key);
			var participants = await securityRule.GetParticipants(key.EntityId);

			var model = securityRule.CanViewConversation(key.EntityId)
				? new Conversation
				{
					Key = conversation.Key,
					Id = conversation.Id,
					CreatedOn = conversation.CreatedOn,
					Comments = conversation.Comments.Select(t => GetComments(t, this.applicationContext, this.userContext)),
					CanAddComments = securityRule.CanAddNewComments(key.EntityId),
					Participants = participants?.Participants,
					UserName = this.userContext.User.UserName
				}
				: null;

			return new Response
			{
				Conversation = model
			};
		}

		public class Response : FormResponse<MyFormResponseMetadata>
		{
			[OutputField(Label = "")]
			public Conversation Conversation { get; set; }
		}

		public class Request : IRequest<Response>
		{
			[InputField(Required = true, Hidden = true)]
			public string Key { get; set; }
		}
	}
}