namespace UiMFTemplate.Conversations.Mappings
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using UiMFTemplate.Conversations.Domain;

	public class CommentMap<TAuthorKey> : IEntityTypeConfiguration<Comment<TAuthorKey>>
	{
		private readonly string schema;

		public CommentMap(string schema)
		{
			this.schema = schema;
		}

		public void Configure(EntityTypeBuilder<Comment<TAuthorKey>> entity)
		{
			entity.ToTable("Comment", this.schema);
			entity.HasKey(t => t.Id);
			entity.Property(t => t.Id).HasColumnName("Id").UseSqlServerIdentityColumn();
			entity.Property(t => t.AuthorId).HasColumnName("AuthorId");
			entity.Property(t => t.PostedOn).HasColumnName("PostedOn");
			entity.Property(t => t.Text).HasColumnName("Text").IsUnicode().IsRequired();
			entity.Property(t => t.ParentId).HasColumnName("ParentId");
			entity.Property(t => t.ConversationId).HasColumnName("ConversationId");

			entity.EnumerableNavigationProperty(
				nameof(Comment<TAuthorKey>.Children),
				Comment<TAuthorKey>.ChildrenFieldName);

			entity.HasMany(t => t.Children)
				.WithOne()
				.HasForeignKey(t => t.ParentId);
		}
	}
}