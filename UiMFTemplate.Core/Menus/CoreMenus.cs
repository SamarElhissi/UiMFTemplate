namespace UiMFTemplate.Core.Menus
{
	using System.Collections.Generic;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms.Menu;

	[RegisterEntry("core")]
	public sealed class CoreMenus : MenuContainer
	{
		public const string Notifications = "Notifications";
		public const string Metrics = "Metrics";
		public const string Settings = "Settings";
		public const string Dashboard = "Dashboard";

		public override IEnumerable<MenuItem> GetDynamicMenuItems()
		{
			yield break;
		}

		public override IEnumerable<MenuGroup> GetMenuGroups()
		{
			return new List<MenuGroup>
			{
				new MenuGroup(Notifications, 200),
				new MenuGroup(Metrics, 100),
				new MenuGroup(Settings, 10),
				new MenuGroup(Dashboard, 120)
			};
		}
	}
}
