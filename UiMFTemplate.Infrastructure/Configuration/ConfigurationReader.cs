namespace UiMFTemplate.Infrastructure.Configuration
{
	using System;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;

	public static class ConfigurationReader
	{
		public static DbContextOptions DbContextOptions(this IConfiguration configRoot)
		{
			var connectionString = configRoot.GetConnectionString("UiMFTemplate");
			return new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
		}

		public static IConfiguration GetConfigurations()
		{
			return new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("appsettings.json")
				.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
				.AddEnvironmentVariables()
				.Build();
		}
	}
}
