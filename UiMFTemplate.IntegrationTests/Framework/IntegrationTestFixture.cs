namespace UiMFTemplate.IntegrationTests.Framework
{
	using System;
	using UiMFTemplate.DataSeed;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Configuration;

	public class IntegrationTestFixture : IDisposable
	{
		public readonly DataSeedDiContainer Container;

		public IntegrationTestFixture()
		{
			var dbContextOptions = ConfigurationReader.GetConfigurations().DbContextOptions();

			using (var connection = dbContextOptions.GetConnection())
			{
				connection.Open();
				Database.TruncateDatabase(connection).Wait();
			}

			this.Container = new DataSeedDiContainer(dbContextOptions);
			this.Container.Container.GetInstance<DataSeed>().SeedRequiredData().Wait();
		}

		public void Dispose()
		{
		}
	}
}
