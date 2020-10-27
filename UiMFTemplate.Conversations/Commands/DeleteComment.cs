namespace UiMFTemplate.Conversations.Commands
{
	using System.Threading;
	using System.Threading.Tasks;
	using MediatR;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMFTemplate.Conversations.Domain;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.User;

	[MyForm(Id = "delete-comment")]
	public class DeleteComment : MyAsyncForm<DeleteComment.Request, DeleteComment.Response>
	{
		private readonly ConversationsDbContext<int> context;
		private readonly ConversationManagerCollection conversationManagerCollection;
		private readonly UserContext userContext;

		public DeleteComment(ConversationsDbContext<int> context,
			UserContext userContext,
			ConversationManagerCollection conversationManagerCollection)
		{
			this.context = context;
			this.userContext = userContext;
			this.conversationManagerCollection = conversationManagerCollection;
		}

		public override async Task<Response> Handle(Request message, CancellationToken cancellationToken)
		{
			var comment = await this.context.Comments.FindOrExceptionAsync(message.Id);
			var conversation = await this.context.Conversations.FindOrExceptionAsync(comment.ConversationId);
			var key = ConversationKey.Parse(conversation.Key);
			var manager = this.conversationManagerCollection.GetInstance(key.EntityType);

			if (comment.AuthorId.ToString() != this.userContext.User.UserId)
			{
				throw new PermissionException("Delete conversation comment", this.userContext);
			}

			this.context.Comments.Remove(comment);
			await this.context.SaveChangesAsync();

			manager.PostAddComment(key.EntityId);
			return new Response();
		}

		public class Request : IRequest<Response>
		{
			[InputField(Required = true, Hidden = true)]
			public int Id { get; set; }

			[InputField(Required = true, Hidden = true)]
			public string Key { get; set; }
		}

		public class Response : FormResponse<MyFormResponseMetadata>
		{
		}
	}
}