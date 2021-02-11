namespace UiMFTemplate.Conversations
{
	using Microsoft.EntityFrameworkCore;
	using UiMFTemplate.Conversations.Domain;

	public class ConversationsDbContext<TAuthorKey>
		: ConversationsDbContext<
			TAuthorKey,
			Conversation<TAuthorKey, Comment<TAuthorKey>>,
			Comment<TAuthorKey>>
	{
		public ConversationsDbContext(DbContextOptions coreDbContextOptions,
			string cnv) : base(coreDbContextOptions, cnv)
		{
		}
	}
}
