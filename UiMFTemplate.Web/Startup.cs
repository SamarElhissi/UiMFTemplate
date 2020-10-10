namespace UiMFTemplate.Web
{
	using System;
	using System.Reflection;
	using Lamar;
	using MediatR;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Microsoft.Extensions.Logging.Abstractions;
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.DependencyInjection;
	using UiMFTemplate.Filing;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.Infrastructure.DataAccess;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Notifications;
	using UiMFTemplate.Users;
	using UiMFTemplate.Web.Email;
	using UiMFTemplate.Web.Middleware;
	using UiMFTemplate.Web.Users;
	using UiMFTemplate.Web.Users.Commands;

	public class Startup
	{
		public const string CorsAllowAllPolicy = "AllowAll";

		public Startup(IConfiguration configuration)
		{
			this.Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseMiddleware(typeof(ErrorHandlingMiddleware));
			app.UseStaticFiles();
			app.UseAuthentication();
			app.UseResponseCaching();

			app.UseRouting();
			app.UseAuthorization();
			app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseSession();
		}

		public void ConfigureContainer(ServiceRegistry registry)
		{
			registry.ConfigureInfrastructure();

			registry.ConfigureAppNotifications();
			ConfigureIdentity(registry);

			registry.ConfigureMediatr();
			registry.AddMediatR(typeof(Login).Assembly);

			// Data access.
			var coreDbContextOptions = this.Configuration.DbContextOptions();
			registry.ConfigureDataAccess(coreDbContextOptions);

			registry.ConfigureUserRoleCheckers();
			registry.ConfigureEmailTemplates();
			registry.ConfigureEmailSenders<AuthMessageSender, AuthMessageSender>();

			registry.For<UserContextAccessor>().Use<AppUserContextAccessor>();
			registry.For<UserContext>().Use(ctx => ctx.GetInstance<UserContextAccessor>().GetUserContext());
			registry.For<UserSession>().Use(ctx => ctx.GetInstance<CookieManager>().GetUserSession()).Transient();

			registry.Scan(_ =>
			{
				_.AssemblyContainingType<CoreDbContext>();
				_.AssemblyContainingType<IEntityFileManager>();
				_.AssemblyContainingType<IMediator>();
				_.AssemblyContainingType<BaseDbContext>();

				_.AssemblyContainingType<INotificationManager>();
				_.AssemblyContainingType<ApplicationDbContext>();
				_.AssemblyContainingType<UserRoleChecker>();

				_.AddAllTypesOf<IAssemblyBootstrapper>();
				_.AddAllTypesOf(typeof(Register<>));
				_.WithDefaultConventions();
			});

			var container = new Container(registry);

			registry.ConfigureDomainEvents(Assembly.GetExecutingAssembly(), container);
			container.RunAssemblyBootstrapers();
			registry.ConfigureRegisters(container);
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
			services.AddRazorPages();

			var configuration = ConfigurationReader.GetConfigurations();

			services.ConfigureMvc(configuration);
			services.ConfigureOptions(configuration);

			services.AddHttpContextAccessor();
			services.AddResponseCaching();
			services.AddHttpsRedirection(options => { options.HttpsPort = 443; });
		}

		private static void ConfigureIdentity(ServiceRegistry registry)
		{
			registry.For<ILogger<SignInManager<ApplicationUser>>>().Use<NullLogger<SignInManager<ApplicationUser>>>();
			registry.For<ILogger<UserManager<ApplicationUser>>>().Use<NullLogger<UserManager<ApplicationUser>>>();
			registry.For<ILogger<RoleManager<ApplicationRole>>>().Use<NullLogger<RoleManager<ApplicationRole>>>();
			registry
				.AddIdentity<ApplicationUser, ApplicationRole>(
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

						// Tokens.
						options.Tokens.EmailConfirmationTokenProvider = nameof(DataProtectorTokenProvider<ApplicationUser>);
					})
				.AddRoles<ApplicationRole>()
				.AddRoleManager<RoleManager<ApplicationRole>>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders()
				.AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(
					nameof(DataProtectorTokenProvider<ApplicationUser>));

			registry.ConfigureApplicationCookie(
				options =>
				{
					// Cookie settings
					options.ExpireTimeSpan = TimeSpan.FromDays(150);
					options.LoginPath = "/Account/LogIn";
					options.LogoutPath = "/Account/LogOut";
				});

			registry.Configure<DataProtectionTokenProviderOptions>(
				options =>
				{
					// Email confirmation link will have a lifespan of 30 days.
					options.TokenLifespan = TimeSpan.FromDays(30);
				});

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