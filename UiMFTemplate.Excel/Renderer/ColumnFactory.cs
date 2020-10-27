namespace UiMFTemplate.Excel.Renderer
{
	using System.Collections.Generic;
	using ExcelExporter.Core;
	using UiMetadataFramework.Core;

	public abstract class ColumnFactory
	{
		protected ColumnFactory(string name)
		{
			this.Name = name;
		}

		public string Name { get; }

		public abstract IEnumerable<Column<object>> GetColumn(string field, OutputFieldMetadata outputFieldMetadata, IList<object> data);
	}
}