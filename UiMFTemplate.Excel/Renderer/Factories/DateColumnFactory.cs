namespace UiMFTemplate.Excel.Renderer.Factories
{
	using System;
	using System.Collections.Generic;
	using ExcelExporter.Core;
	using UiMetadataFramework.Core;

	public class DateColumnFactory : ColumnFactory
	{
		public DateColumnFactory() : base("datetime")
		{
		}

		public override IEnumerable<Column<object>> GetColumn(string field, OutputFieldMetadata outputFieldMetadata, IList<object> data)
		{
			yield return new Column<object>(
				outputFieldMetadata.Label,
				row =>
				{
					var value = row.GetPropertyValue(field);

					if (value == null)
					{
						return new CellData(null);
					}

					return new CellData(Convert.ToDateTime(row.GetPropertyValue(field))) { NumberFormat = "yyyy-MM-dd HH:mm:ss" };
				});
		}
	}
}
