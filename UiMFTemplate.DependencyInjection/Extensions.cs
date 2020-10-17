namespace UiMFTemplate.DependencyInjection
{
	using System;
	using System.Linq;
	using System.Reflection;
	using Filer.Core;
	using Filer.EntityFrameworkCore;
	using Lamar;
	using MediatR;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Options;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;
	using UiMFTemplate.Conversations;
	using UiMFTemplate.Core.DataAccess;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.Infrastructure.Decorators;
	using UiMFTemplate.Infrastructure.Domain;
	using UiMFTemplate.Infrastructure.Emails;
	using UiMFTemplate.Infrastructure.Forms.Menu;
	using UiMFTemplate.Infrastructure.Messages;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Notifications;
	using UiMFTemplate.Users;
	using UiMFTemplate.Users.CustomTokenProvider;
	using UimfDependencyInjectionContainer = UiMetadataFramework.Core.Binding.DependencyInjectionContainer;
	using AppDependencyInjectionContainer = UiMFTemplate.Infrastructure.DependencyInjectionContainer;

	public static class Extensions
	{
		private static readonly Lazy<Assembly[]> AssembliesWithBootstrapper = new Lazy<Assembly[]>(GetAssembliesWithBootstrapper);
		private static readonly Lazy<Assembly[]> AssembliesWithRequestHandlers = new Lazy<Assembly[]>(GetAssembliesWithRequestHandlers);
		public static IdentityBuilder AddEmailConfirmationTotpTokenProvider(this IdentityBuilder builder)
		{
			var userType = builder.UserType;
			var totpProvider = typeof(EmailTotpTokenProvider<>).MakeGenericType(userType);
			return builder.AddTokenProvider("EmailTotpTokenProvider", totpProvider);
		}

		public static void ConfigureAppNotifications(this ServiceRegistry registry)
		{
			registry.For<App.EventNotification.Bootstrap>();
		}

		public static void ConfigureDataAccess(this ServiceRegistry registry, DbContextOptions coreDbContextOptions)
		{
			registry.For<DbContextOptions>().Use(coreDbContextOptions);

			// Core
			registry.For<CoreDbContext>().Use(t => new CoreDbContext(
				coreDbContextOptions,
				t.GetInstance<EventManager>(),
				t.GetInstance<UserSession>(),
				t.GetInstance<IOptions<AppConfig>>()));

			// Users
			registry.For<ApplicationDbContext>().Use(ctx => new ApplicationDbContext(coreDbContextOptions));

			// Notifications
			registry.For<NotificationsDbContext>().Use(t => new NotificationsDbContext(coreDbContextOptions, "ntf"));

			// Filer
			registry.For<IFileManager>().Use<FileManager>();
			registry.For<DataContext>().Use(t => new DataContext(coreDbContextOptions));

			// Unops.Gms.Conversations
			registry.For<ConversationsDbContext<int>>().Use(ctx => new ConversationsDbContext<int>(coreDbContextOptions, "cnv"));

		}

		public static void ConfigureDomainEvents(this ServiceRegistry registry, Assembly callingAssembly, Container container)
		{
			var di = new AppDependencyInjectionContainer(t => container.GetNestedContainer().GetInstance(t));
			registry.For<EventManager>().Use(t => new EventManager(di)).Singleton();

			// Set unique lifecycle for all AppEventHandler<> classes
			registry.SetLifecycleForImplementationsOfGenericType(
				typeof(AppEventHandler<>),
				callingAssembly, // The application layer (i.e. - web or test).
				typeof(EventManager).Assembly,
				typeof(CoreDbContext).Assembly);
		}

		public static void ConfigureEmailSenders<TEmailSender, TSmsSender>(this ServiceRegistry registry)
			where TEmailSender : IEmailSender
			where TSmsSender : ISmsSender
		{
			registry.ConfigureEmailSenders(typeof(TEmailSender), typeof(TSmsSender));
		}

		public static void ConfigureEmailSenders(this ServiceRegistry registry, Type emailSenderType, Type smsSenderType)
		{
			var container = new Container(registry);
			registry.For<IEmailSender>().Use(t => (IEmailSender)container.GetInstance(emailSenderType));
			registry.For<ISmsSender>().Use(t => (ISmsSender)container.GetInstance(smsSenderType));
		}

		public static void ConfigureEmailTemplates(this ServiceRegistry registry)
		{
			registry.For<IViewRenderService>().Use<ViewRenderService>();
			registry.For<EmailTemplateRegister>().Use(ctx => new EmailTemplateRegister(ctx.GetInstance<IEmailSender>(), ctx.GetInstance<AppDependencyInjectionContainer>())).Singleton();
			registry.SetLifecycleForImplementationsOfInterface(typeof(IEmailTemplate<>), AssembliesWithBootstrapper.Value);
		}

		public static void ConfigureInfrastructure(this ServiceRegistry registry)
		{
			registry.For<MetadataBinder>().Use(GetMetadataBinder).Singleton();
			registry.For<FormRegister>().Use(ctx => new FormRegister(ctx.GetInstance<MetadataBinder>())).Singleton();

			registry.AddSingleton<MenuRegister>();
			registry.For<ActionRegister>().Use(ctx => ActionRegister.Default).Singleton();

			registry.AddTransient<IEntityRepository>();
			registry.For<EntitySecurityConfigurationRegister>().Use(ctx => new EntitySecurityConfigurationRegister(new AppDependencyInjectionContainer(ctx.GetInstance))).Singleton();
			registry.For<ObjectSecurityConfigurationRegister>().Use(ctx => new ObjectSecurityConfigurationRegister()).Singleton();

			registry.For<AppDependencyInjectionContainer>().Use(ctx => new AppDependencyInjectionContainer(ctx.GetInstance));
			registry.For<UimfDependencyInjectionContainer>().Use(t => new UimfDependencyInjectionContainer(t.GetInstance));
		}

		public static void ConfigureMediatr(this ServiceRegistry registry)
		{
			registry.AddMediatR(AssembliesWithRequestHandlers.Value);
			registry.For(typeof(IPipelineBehavior<,>)).Add(typeof(SecurityDecorator<,>));
		}

		public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddOptions();
			services.Configure<AppConfig>(a => configuration.GetSection("AppConfig"));
		}

		public static void ConfigureRegisters(this ServiceRegistry serviceRegistry, Container container)
		{
			var registerTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(s => s.GetLoadableTypes())
				.Where(p => p.ImplementsGenericType(typeof(Register<>)) && p != typeof(Register<>))
				.ToList();

			var assembliesToRegister = AppDomain.CurrentDomain
				.GetAssemblies()
				.Where(a => !a.IsDynamic)
				.ToList();

			foreach (var registerType in registerTypes)
			{
				var register = container.GetInstance(registerType);
				var registerAssemblyMethod = registerType.GetMethod(nameof(Register<int>.RegisterAssembly));

				foreach (var assembly in assembliesToRegister)
				{
					// ReSharper disable once PossibleNullReferenceException
					registerAssemblyMethod.Invoke(register, new object[] { assembly });
				}

				serviceRegistry.AddSingleton(registerType, register);
			}
		}

		public static void ConfigureUserRoleCheckers(this ServiceRegistry registry)
		{
			var child = (Container)new Container(registry).GetNestedContainer();

			var serviceRegistry = new ServiceRegistry();

			serviceRegistry.AddTransient<CoreDbContext>();
			serviceRegistry.AddTransient<IFileManager>();
			serviceRegistry.AddTransient<ApplicationDbContext>();

			child.Configure(serviceRegistry);

			registry.For<UserRoleCheckerRegister>()
				.Use(t => new UserRoleCheckerRegister(new AppDependencyInjectionContainer(x => child.GetInstance(x))))
				.Singleton();

			registry.SetLifecycleForImplementationsOfInterface(typeof(IUserRoleChecker), AssembliesWithBootstrapper.Value);
		}

		public static void RunAssemblyBootstrapers(this IContainer container)
		{
			foreach (var bootstrapper in container.GetAllInstances<IAssemblyBootstrapper>().OrderByDescending(t => t.Priority))
			{
				bootstrapper.Start(new AppDependencyInjectionContainer(container.GetInstance));
			}
		}

		public static void SetLifecycleForImplementationsOfGenericType(
			this ServiceRegistry registry,
			Type genericType,
			params Assembly[] scanAssembliesContainingTypes)
		{
			scanAssembliesContainingTypes
				.Distinct()
				.SelectMany(t => t.ExportedTypes)
				.Where(t => t.ImplementsGenericType(genericType))
				.ToList()
				.ForEach(t => registry.AddTransient(t));
		}

		public static void SetLifecycleForImplementationsOfInterface(
			this ServiceRegistry registry,
			Type genericType,
			params Assembly[] scanAssembliesContainingTypes)
		{
			scanAssembliesContainingTypes
				.Distinct()
				.SelectMany(t => t.ExportedTypes)
				.Where(genericType.IsAssignableFrom)
				.ToList()
				.ForEach(t => registry.AddTransient(t));
		}

		private static Assembly[] GetAssembliesWithBootstrapper()
		{
			var mainAssemblies = new[]
			{
				typeof(Core.Bootstrap).Assembly,
				typeof(App.EventNotification.Bootstrap).Assembly,
				typeof(Filing.Bootstrap).Assembly,
				typeof(Help.Bootstrap).Assembly,
				typeof(Users.Bootstrap).Assembly,
				typeof(Infrastructure.Bootstrap).Assembly,
				typeof(Conversations.Bootstrap).Assembly
			};

			// Load all referenced assemblies that belong to the solution.
			Assembly
				.GetEntryAssembly()
				.GetReferencedAssemblies()
				.Where(t => t.FullName.Contains("UiMFTemplate"))
				.ForEach(t => Assembly.Load(t));

			return AppDomain.CurrentDomain
				.GetAssemblies()
				.Where(t => !mainAssemblies.Contains(t))
				.Where(assembly => assembly.GetTypes().Any(t => typeof(IAssemblyBootstrapper).IsAssignableFrom(t)))
				.Union(mainAssemblies)
				.Distinct()
				.ToArray();
		}

		/// <summary>
		/// Gets all referenced (directly and indirectly) assemblies which contain implementations
		/// of <see cref="IRequestHandler{TRequest,TResponse}"/>.
		/// </summary>
		/// <returns>List of matching assemblies.</returns>
		/// <remarks></remarks>
		private static Assembly[] GetAssembliesWithRequestHandlers()
		{
			var mainAssemblies = new[]
			{
				// Third-party assemblies with `IRequestHandler`s
				typeof(InvokeForm).Assembly,

				// App's assemblies with `IRequestHandler`s
				typeof(Core.Bootstrap).Assembly,
				typeof(App.EventNotification.Bootstrap).Assembly,
				typeof(Filing.Bootstrap).Assembly,
				typeof(Help.Bootstrap).Assembly,
				typeof(Users.Bootstrap).Assembly,
				typeof(Infrastructure.Bootstrap).Assembly,
				typeof(Conversations.Bootstrap).Assembly
			};

			// Load all referenced assemblies that belong to the solution.
			Assembly
				.GetEntryAssembly()
				.GetReferencedAssemblies()
				.Where(t => t.FullName.Contains("UiMFTemplate"))
				.ForEach(t => Assembly.Load(t));

			// Try find other assemblies that might have `IRequestHandler`s
			return AppDomain.CurrentDomain
				.GetAssemblies()
				.Where(t => !mainAssemblies.Contains(t))
				.Where(assembly => assembly.GetTypes().Any(t => t.ImplementsGenericType(typeof(IRequestHandler<,>))))
				.Union(mainAssemblies)
				.Distinct()
				.ToArray();
		}

		private static MetadataBinder GetMetadataBinder(IServiceContext context)
		{
			var binder = new MetadataBinder(context.GetInstance<UimfDependencyInjectionContainer>());
			binder.RegisterAssembly(typeof(StringInputFieldBinding).Assembly);
			return binder;
		}
	}
}
