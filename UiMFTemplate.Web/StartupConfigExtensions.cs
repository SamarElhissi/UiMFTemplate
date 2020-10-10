namespace UiMFTemplate.Web
{
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Converters;
	using Newtonsoft.Json.Serialization;
	using UiMFTemplate.Infrastructure.Configuration;

	public static class StartupConfigExtensions
	{
		public static void ConfigureMvc(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddMvc(
					options =>
					{
						options.EnableEndpointRouting = false;

					})
				.AddNewtonsoftJson(
					options =>
					{
						options.SerializerSettings.Converters.Add(new StringEnumConverter());
						options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
						options.SerializerSettings.ContractResolver = new DefaultContractResolver
						{
							NamingStrategy = new CamelCaseNamingStrategy
							{
								ProcessDictionaryKeys = true,
								OverrideSpecifiedNames = false
							}
						};
					});

			// Enable session-state.
			services.AddDistributedMemoryCache();
			services.AddSession();

			// Configure options from appsettings.json.
			services.AddOptions();
			services.Configure<AppConfig>(configuration.GetSection("AppConfig"));

			services.AddCors(o => o.AddPolicy(Startup.CorsAllowAllPolicy, builder =>
			{
				builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
			}));
		}
	}
}
