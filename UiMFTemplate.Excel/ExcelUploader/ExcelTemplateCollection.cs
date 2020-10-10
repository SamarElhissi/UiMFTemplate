namespace UiMFTemplate.Excel.ExcelUploader
{
	using UiMFTemplate.Infrastructure;

	public class ExcelTemplateCollection : Register<IExcelTemplate>
	{
		public ExcelTemplateCollection(DependencyInjectionContainer dependencyInjectionContainer) : base(dependencyInjectionContainer)
		{
		}
	}
}
