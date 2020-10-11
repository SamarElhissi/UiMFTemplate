namespace UiMFTemplate.DataSeed
{
	using System;
	using System.Reflection;
	using Lamar;
	using MediatR;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.InMemory.Infrastructure.Internal;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Logging.Abstractions;
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.DependencyInjection;
	using UiMFTemplate.Filing;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.DataAccess;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Notifications;
	using UiMFTemplate.Users;

	public class DataSeedDiContainer
	{
		//public readonly ManuallyControlledLifecycle CoreDbContextLifecycle = new ManuallyControlledLifecycle();
		public readonly bool UsingInMemoryDatabase;
		public readonly Container Container;

		/// <summary>
		/// Initializes a new instance of the <see cref="DataSeedDiContainer"/> class,
		/// which is preconfigured to use a real database specified with the connection
		/// string.
		/// </summary>
		public DataSeedDiContainer(string connectionString)
			: this(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataSeedDiContainer"/> class,
		/// which is preconfigured to use an in-memory-database.
		/// </summary>
		public DataSeedDiContainer()
			: this(new DbContextOptionsBuilder().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options)
		{
		}

		public DataSeedDiContainer(DbContextOptions dbContextOptions)
		{
			var registry = new ServiceRegistry();

			this.UsingInMemoryDatabase = dbContextOptions.FindExtension<InMemoryOptionsExtension>() != null;

			registry.ConfigureInfrastructure();

			registry.ConfigureAppNotifications();
			ConfigureIdentity(registry);

			registry.ConfigureMediatr();

			// Data access.
			registry.ConfigureDataAccess(dbContextOptions);

			registry.ConfigureUserRoleCheckers();
			registry.ConfigureEmailTemplates();
			registry.ConfigureEmailSenders<FakeMessageSender, FakeMessageSender>();

			registry.For<UserContextAccessor>().Use<AppUserContextAccessor>();
			registry.For<UserContext>().Use(ctx => ctx.GetInstance<UserContextAccessor>().GetUserContext());

			registry.For<UserSession>().Use(t => this.CurrentUserSession).Transient();

			registry.Scan(_ =>
			{
				_.AssemblyContainingType<CoreDbContext>();
				_.AssemblyContainingType<IEntityFileManager>();
				_.AssemblyContainingType<IMediator>();
				_.AssemblyContainingType<BaseDbContext>();

				_.AssemblyContainingType<INotificationManager>();
				_.AssemblyContainingType<ApplicationDbContext>();

				_.AddAllTypesOf<IAssemblyBootstrapper>();
				_.AddAllTypesOf(typeof(Register<>));
				_.WithDefaultConventions();
			});

			var container = new Container(registry);

			registry.ConfigureDomainEvents(Assembly.GetExecutingAssembly(), container);
			container.RunAssemblyBootstrapers();
			registry.ConfigureRegisters(container);
			this.Container = new Container(registry);
		}

		/// <summary>
		/// Gets or sets user session configured for this container.
		/// </summary>
		public UserSession CurrentUserSession { get; set; }


		private static void ConfigureIdentity(ServiceRegistry registry)
		{
			//registry.For<ILogger<SignInManager<ApplicationUser>>>().Use<NullLogger<SignInManager<ApplicationUser>>>();
			registry.For<ILogger<UserManager<ApplicationUser>>>().Use<NullLogger<UserManager<ApplicationUser>>>();
			registry.For<ILogger<RoleManager<ApplicationRole>>>().Use<NullLogger<RoleManager<ApplicationRole>>>();
			registry.AddIdentityCore<ApplicationUser>(
					options =>
					{
						// Password settings
						options.Password.RequireDigit = true;
						options.Password.RequiredLength = 8;
						options.Password.RequireNonAlphanumeric = false;
						options.Password.RequireUppercase = true;
						options.Password.RequireLowercase = false;

						// Lockout settings
						options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
						options.Lockout.MaxFailedAccessAttempts = 10;

						// User settings
						options.User.RequireUniqueEmail = true;

					})
				.AddRoles<ApplicationRole>()
				.AddRoleManager<RoleManager<ApplicationRole>>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			registry.Configure<IdentityOptions>(options =>
			{
				// Password settings
				options.Password.RequireDigit = false;
				options.Password.RequiredLength = 1;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				// Lockout settings
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
				options.Lockout.MaxFailedAccessAttempts = 10;

				// User settings
				options.User.RequireUniqueEmail = false;

				options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+'#!/^%{}* ";
			});
		}

	}
}
