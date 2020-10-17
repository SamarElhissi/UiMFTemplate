namespace UiMFTemplate.Core.NotificationManagers
{
	using System;
	using UiMFTemplate.Core.Commands;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Notifications;

	[RegisterEntry(Key)]
	public class MagicNotificationManager : INotificationManager
	{
		public const string Key = "UiMFTemplate.Core.Domain.Magic";
		public NotificationDetail GetLink(object entityId)
		{
			return new NotificationDetail
			{
				Link = MagicDetails.Button(Convert.ToInt32(entityId))
			};
		}
	}
}
