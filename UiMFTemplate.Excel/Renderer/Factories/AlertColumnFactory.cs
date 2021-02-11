namespace UiMFTemplate.Excel.Renderer.Factories
{
	using System.Collections.Generic;
	using ExcelExporter.Core;
	using UiMetadataFramework.Core;
	using UiMFTemplate.Infrastructure.Forms.Outputs;

	public class AlertColumnFactory : ColumnFactory
	{
		public AlertColumnFactory() : base("alert")
		{
		}

		public override IEnumerable<Column<object>> GetColumn(string field, OutputFieldMetadata outputFieldMetadata, IList<object> data)
		{
			yield return new Column<object>(
				outputFieldMetadata.Label,
				row =>
				{
					var value = row.GetPropertyValue(field);

					if (!(value is Alert alert))
					{
						return new CellData(null);
					}

					return new CellData(alert.Heading);
				});
		}
	}
}
