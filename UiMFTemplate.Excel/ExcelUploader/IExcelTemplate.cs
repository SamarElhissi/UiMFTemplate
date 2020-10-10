namespace UiMFTemplate.Excel.ExcelUploader
{
	using System.Collections.Generic;

	public interface IExcelTemplate
	{
		List<string> GetRequiredColumns();
	}
}
