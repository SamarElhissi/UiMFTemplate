namespace UiMFTemplate.DataSeed.Seeders
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;
	using Lamar;
	using Microsoft.Extensions.DependencyInjection;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Infrastructure;

	public static class Extensions
	{
		public static T Do<T>(this T tester, string user, Action<T> action)
			where T : Seeder
		{
			var previousSession = tester.UserSession;
			tester.LoginAs(user);

			action(tester);

			tester.UserSession = previousSession;

			return tester;
		}

		public static async Task<T> Do<T>(this T tester, string user, Func<T, Task> action)
			where T : Seeder
		{
			var previousSession = tester.UserSession;
			tester.LoginAs(user);

			await action(tester);

			tester.UserSession = previousSession;

			return tester;
		}

		public static T LoginAs<T>(this T tester, string user)
			where T : Seeder
		{
			var userId = tester.User(user).GetEntity().Id;
			var session = new UserSession(userId.ToString());
			tester.UserSession = session;

			return tester;
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
	}
}
