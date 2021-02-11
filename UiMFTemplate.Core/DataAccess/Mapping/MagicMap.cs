namespace UiMFTemplate.Core.DataAccess.Mapping
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using UiMFTemplate.Core.Domain;

	public class MagicMap : IEntityTypeConfiguration<Magic>
	{
		public void Configure(EntityTypeBuilder<Magic> entity)
		{
			entity.ToTable("Magic");
			entity.HasKey(c => c.Id);
		}
	}
}
