namespace UiMFTemplate.Notifications
{
	public interface INotificationManager
	{
		NotificationDetail GetLink(object entityId);
	}
}