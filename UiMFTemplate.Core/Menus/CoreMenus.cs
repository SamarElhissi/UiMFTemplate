namespace UiMFTemplate.Core.Menus
{
	using System.Collections.Generic;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms.Menu;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;
	using UiMFTemplate.Notifications;

	[RegisterEntry("core")]
	public sealed class CoreMenus : MenuContainer
	{
		public const string Notifications = "Notifications";
		public const string Orders = "Orders";
		public const string Invoices = "Invoices";
		public const string Settings = "Settings";
		public const string Dashboard = "Dashboard";

		private readonly NotificationsDbContext context;
		private readonly UserContext userContext;
		private readonly UserSecurityContext userSecurityContext;

		public CoreMenus(NotificationsDbContext context, UserSecurityContext userSecurityContext, UserContext userContext)
		{
			this.context = context;
			this.userSecurityContext = userSecurityContext;
			this.userContext = userContext;
		}

		public override IEnumerable<MenuItem> GetDynamicMenuItems()
		{
			yield break;
		}

		public override IEnumerable<MenuGroup> GetMenuGroups()
		{
			return new List<MenuGroup>
			{
				new MenuGroup(Notifications, 200),
				new MenuGroup(Orders, 100),
				new MenuGroup(Invoices, 110),
				new MenuGroup(Settings, 10),
				new MenuGroup(Dashboard, 120)
			};
		}
	}
}