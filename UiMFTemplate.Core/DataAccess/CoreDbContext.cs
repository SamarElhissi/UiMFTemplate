namespace UiMFTemplate.Core.DataAccess
{
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Options;
	using UiMFTemplate.Core.DataAccess.Mapping;
	using UiMFTemplate.Core.Domain;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.Infrastructure.DataAccess;
	using UiMFTemplate.Infrastructure.Domain;
	using UiMFTemplate.Infrastructure.User;

	public class CoreDbContext : BaseDbContext
	{
		public CoreDbContext(DbContextOptions options, EventManager eventManager, UserSession userSession, IOptions<AppConfig> appConfig)
			: base(options, eventManager, appConfig, userSession)
		{
		}

		public virtual DbSet<Magic> Magics { get; set; }

		public virtual DbSet<RegisteredUser> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.ApplyConfiguration(new RegisteredUserMap());
			builder.ApplyConfiguration(new RegisteredRoleMap());
			builder.ApplyConfiguration(new RegisteredUserRoleMap());
			builder.ApplyConfiguration(new MagicMap());
		}
	}
}
