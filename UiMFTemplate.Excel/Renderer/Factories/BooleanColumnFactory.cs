namespace UiMFTemplate.Excel.Renderer.Factories
{
	using System.Collections.Generic;
	using ExcelExporter.Core;
	using UiMetadataFramework.Core;
	using UiMFTemplate.Infrastructure;

	public class BooleanColumnFactory : ColumnFactory
	{
		public BooleanColumnFactory() : base("boolean")
		{
		}

		public override IEnumerable<Column<object>> GetColumn(string field, OutputFieldMetadata outputFieldMetadata, IList<object> data)
		{
			yield return new Column<object>(
				outputFieldMetadata.Label,
				row =>
				{
					var value = row.GetPropertyValue(field);

					if (!(value is bool boolean))
					{
						return new CellData(null);
					}

					return new CellData(boolean.ToYesOrNoString());
				});
		}
	}
}
