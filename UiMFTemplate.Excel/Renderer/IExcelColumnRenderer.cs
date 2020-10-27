namespace UiMFTemplate.Excel.Renderer
{
	using System.Collections.Generic;
	using ExcelExporter.Core;
	using UiMetadataFramework.Core;

	public interface IExcelColumnRenderer
	{
		IEnumerable<Column<object>> GetColumns(string field, OutputFieldMetadata outputFieldMetadata, IList<object> data);
	}
}