namespace UiMFTemplate.Infrastructure.Emails
{
	using System.Reflection;
	using System.Threading.Tasks;
	using RazorLight;
	using UiMFTemplate.Infrastructure.Messages;

	public class ViewRenderService : IViewRenderService
	{
		public async Task<string> RenderToString<TModel>(TModel model)
		{
			var type = model.GetType();
			var typeDeclaringType = type.DeclaringType;

			if (typeDeclaringType == null)
			{
				throw new BusinessException("Invalid attempt to send email.");
			}

			var templateBody = await this.RenderPartialView(
				type.Assembly,
				typeDeclaringType.Namespace + "." + typeDeclaringType.Name + ".cshtml",
				model);

			return templateBody;
		}

		/// <summary>
		/// Renders "cshtml" email template to string.
		/// </summary>
		/// <param name="assembly">Assembly where the embedded resource resides.</param>
		/// <param name="embeddedResourceName"></param>
		/// <param name="model">The model to pass to the view.</param>
		private async Task<string> RenderPartialView<T>(Assembly assembly, string embeddedResourceName, T model)
		{
			var modelType = typeof(T);
			var engine = new RazorLightEngineBuilder()
				.UseEmbeddedResourcesProject(typeof(T))
				.UseMemoryCachingProvider()
				.Build();

			var source = assembly.GetEmbeddedResourceText(embeddedResourceName);
			return await engine.CompileRenderStringAsync(modelType.FullName, source, model);
		}
	}
}
