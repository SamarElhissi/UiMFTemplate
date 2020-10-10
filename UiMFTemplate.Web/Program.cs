namespace UiMFTemplate.Web
{
	using System.IO;
	using Lamar.Microsoft.DependencyInjection;
	using Microsoft.AspNetCore;
	using Microsoft.AspNetCore.Hosting;

	public class Program
	{
		public static void Main(string[] args)
		{
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
				.UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
				.UseStartup<Startup>()
				.UseLamar()
				.Build();
    }
}
