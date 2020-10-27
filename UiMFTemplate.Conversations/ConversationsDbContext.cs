namespace UiMFTemplate.Conversations
{
	using System.Linq;
	using Microsoft.EntityFrameworkCore;
	using UiMFTemplate.Conversations.Domain;
	using UiMFTemplate.Conversations.Mappings;

	public class ConversationsDbContext<TAuthorKey, TConversation, TComment> : DbContext
		where TComment : Comment<TAuthorKey>, new()
		where TConversation : Conversation<TAuthorKey, TComment>, new()
	{
		protected readonly string schema;

		public ConversationsDbContext(DbContextOptions options,
			string schema = "forms") : base(options)
		{
			this.schema = schema;
		}

		public virtual DbSet<TComment> Comments { get; set; }
		public virtual DbSet<TConversation> Conversations { get; set; }

		public void DeleteConversation(int conversationId)
		{
			var conversation = this.Conversations.SingleOrDefault(a => a.Id == conversationId);
			if (conversation != null)
			{
				var comments = this.Comments.Where(a => a.ConversationId == conversation.Id).ToList();
				foreach (var comment in comments)
				{
					this.Comments.Remove(comment);
				}

				this.Conversations.Remove(conversation);
				this.SaveChanges();
			}
		}

		public TConversation EnsureConversation(string key)
		{
			var conversation = this.Conversations.Include(t => t.Comments).SingleOrDefault(c => c.Key == key);

			if (conversation == null)
			{
				conversation = new TConversation();
				conversation.ChangeKey(key);

				this.Conversations.Add(conversation);
				this.SaveChanges();
			}

			return conversation;
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.HasDefaultSchema(this.schema);
			builder.ApplyConfiguration(new CommentMap<int>(this.schema));
			builder.ApplyConfiguration(new ConversationMap<int, Comment<int>>(this.schema));
			base.OnModelCreating(builder);
		}
	}
}