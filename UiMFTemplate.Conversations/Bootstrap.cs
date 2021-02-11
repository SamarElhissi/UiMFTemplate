namespace UiMFTemplate.Conversations
{
	using System.Reflection;
	using UiMFTemplate.Infrastructure;

	public class Bootstrap : IAssemblyBootstrapper
	{
		public int Priority { get; } = 60;

		public void Start(DependencyInjectionContainer dependencyInjectionContainer)
		{
			dependencyInjectionContainer.RegisterUiMetadata(typeof(Bootstrap).GetTypeInfo().Assembly);
		}
	}
}
