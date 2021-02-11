namespace UiMFTemplate.Core.DataAccess.Mapping
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using UiMFTemplate.Core.Domain;

	public class RegisteredUserRoleMap : IEntityTypeConfiguration<RegisteredUserRole>
	{
		public void Configure(EntityTypeBuilder<RegisteredUserRole> entity)
		{
			entity.ToTable("AspNetUserRoles");
			entity.Property(t => t.UserId).HasColumnName("UserId");
			entity.Property(t => t.RoleId).HasColumnName("RoleId");

			entity.HasKey(bc => new { bc.UserId, bc.RoleId });

			entity
				.HasOne(bc => bc.RegisteredUser)
				.WithMany(b => b.RegisteredUserRoles)
				.HasForeignKey(bc => bc.UserId);

			entity
				.HasOne(bc => bc.RegisteredRole)
				.WithMany(b => b.RegisteredUserRoles)
				.HasForeignKey(bc => bc.RoleId);
		}
	}
}
