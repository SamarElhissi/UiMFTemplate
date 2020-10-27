namespace UiMFTemplate.Conversations
{
	using UiMFTemplate.Infrastructure;

	public class ConversationManagerCollection : Register<IConversationManager>
	{
		public ConversationManagerCollection(DependencyInjectionContainer dependencyInjectionContainer) : base(dependencyInjectionContainer)
		{
		}
	}
}