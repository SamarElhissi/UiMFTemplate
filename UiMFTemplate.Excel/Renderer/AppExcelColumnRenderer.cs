namespace UiMFTemplate.Excel.Renderer
{
	using System.Collections.Generic;
	using ExcelExporter.Core;
	using UiMetadataFramework.Core;

	public class AppExcelColumnRenderer : IExcelColumnRenderer
	{
		public IEnumerable<Column<object>> GetColumns(string field, OutputFieldMetadata outputFieldMetadata, IList<object> data)
		{
			if (outputFieldMetadata.CustomProperties != null &&
				outputFieldMetadata.CustomProperties.TryGetValue("hiddenInExcel", out var _))
			{
				yield break;
			}

			var label = outputFieldMetadata.Label;

			var factory = ColumnRegistry.Default.FactoryFor(outputFieldMetadata.Type);

			if (factory != null)
			{
				var columns = factory.GetColumn(field, outputFieldMetadata, data);
				foreach (var column in columns)
				{
					yield return column;
				}
			}

			else if (field != null)
			{
				yield return new Column<object>(
					label,
					row => new CellData(row.GetPropertyValue(field)));
			}
		}
	}
}