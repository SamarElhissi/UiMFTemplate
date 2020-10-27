namespace UiMFTemplate.Core.DataAccess.Mapping
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using UiMFTemplate.Core.Domain;

	public class RegisteredRoleMap : IEntityTypeConfiguration<RegisteredRole>
	{
		public void Configure(EntityTypeBuilder<RegisteredRole> entity)
		{
			entity.ToTable("AspNetRoles");
			entity.HasKey(c => c.Id);
			entity.Property(t => t.Id).HasColumnName("Id").UseSqlServerIdentityColumn();

			entity.Property(t => t.ConcurrencyStamp).HasColumnName("ConcurrencyStamp");
			entity.Property(t => t.Name).HasColumnName("Name").HasMaxLength(250);
			entity.Property(t => t.NormalizedName).HasColumnName("NormalizedName").HasMaxLength(250);
		}
	}
}