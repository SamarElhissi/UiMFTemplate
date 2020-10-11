namespace UiMFTemplate.DataSeed
{
	using System;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.DataSeed.Seeds;

	internal class Program
	{
		private static void Main()
		{
			var config = ConfigurationReader.GetConfigurations();
			var dbContextOptions = config.DbContextOptions();

			using (var connection = dbContextOptions.GetConnection())
			{
				Console.WriteLine("Deleting old data...");

				connection.Open();
				//Database.TruncateDatabase(connection).Wait();
			}

			Console.WriteLine("Seeding new data...");
			var demo = new Demo(dbContextOptions);
			demo.Run().Wait();

			Console.WriteLine("Data seed has completed successfully. Press any key to exit.");
			Console.ReadKey();
		}
	}
}