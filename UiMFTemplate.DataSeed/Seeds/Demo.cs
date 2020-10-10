namespace UiMFTemplate.DataSeed.Seeds
{
	using System.Threading.Tasks;
	using Microsoft.EntityFrameworkCore;
	using UiMFTemplate.Core.Security;
	using UiMFTemplate.Users.Security;

	public class Demo : Seed
	{
        public Demo(DbContextOptions dbContextOptions) : base(dbContextOptions)
		{
		}

		public override async Task Run()
		{
			var dataSeed = this.Container.Container.GetInstance<DataSeed>();
			dataSeed.SeedRequiredData().Wait();

			await SeedUsers(dataSeed);

		}

		private static async Task SeedUsers(DataSeed dataSeed)
		{
			await dataSeed.EnsureUser("admin@example.com", "Password1", UserManagementRoles.UserAdmin, CoreRoles.SysAdmin);
			await dataSeed.EnsureUser("supervisor@example.com", "Password1", CoreRoles.Supervisor);
        }
    }
}
