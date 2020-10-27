namespace UiMFTemplate.Notifications
{
	using UiMFTemplate.Infrastructure;

	public class NotificationManagerRegister : Register<INotificationManager>
	{
		public NotificationManagerRegister(DependencyInjectionContainer dependencyInjectionContainer) : base(dependencyInjectionContainer)
		{
		}
	}
}