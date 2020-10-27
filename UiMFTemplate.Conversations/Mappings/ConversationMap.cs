namespace UiMFTemplate.Conversations.Mappings
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using UiMFTemplate.Conversations.Domain;

	public class ConversationMap<TAuthorKey, TComment> : IEntityTypeConfiguration<Conversation<TAuthorKey, TComment>>
		where TComment : Comment<TAuthorKey>, new()
	{
		private readonly string schema;

		public ConversationMap(string schema)
		{
			this.schema = schema;
		}

		public void Configure(EntityTypeBuilder<Conversation<TAuthorKey, TComment>> entity)
		{
			entity.ToTable("Conversation", this.schema);
			entity.HasKey(t => t.Id);
			entity.Property(t => t.Id).HasColumnName("Id").UseSqlServerIdentityColumn();
			entity.Property(t => t.CreatedOn).HasColumnName("CreatedOn");
			entity.Property(t => t.Key).HasColumnName("Key").HasMaxLength(Conversation<TAuthorKey, TComment>.KeyMaxLength).IsUnicode(false)
				.IsRequired();

			entity.HasMany(t => t.Comments)
				.WithOne()
				.HasForeignKey(t => t.ConversationId);

			entity.EnumerableNavigationProperty(
				nameof(Conversation<TAuthorKey, TComment>.Comments),
				Conversation<TAuthorKey, TComment>.CommentsFieldName);
		}
	}
}