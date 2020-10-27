namespace UiMFTemplate.Excel
{
	using System.Collections.Generic;
	using System.Linq;
	using ExcelExporter.Core;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;
	using UiMFTemplate.Excel.Renderer;

	public class ExcelEngine
	{
		private readonly IExcelColumnRenderer columnRenderer;

		public ExcelEngine(IExcelColumnRenderer columnRenderer)
		{
			this.columnRenderer = columnRenderer;
		}

		public ExcelFile BuildExcelFile(object data, FormMetadata metadata, string property)
		{
			var columns = new List<Column<object>>();
			IEnumerable<object> array;
			var value = data.GetPropertyValue(property);
			if (value.GetType().GetGenericTypeDefinition() == typeof(PaginatedData<>))
			{
				var paginatedData = value.GetPropertyValue(nameof(PaginatedData<object>.Results));
				array = (IEnumerable<object>)paginatedData;
			}
			else
			{
				array = (IEnumerable<object>)value;
			}

			var fieldMetadata = metadata.OutputFields
				// Only select metadata for properties to be exported.
				.SingleOrDefault(t => t.Id.Equals(property));

			var dataItems = array.ToList();
			if (fieldMetadata != null)
			{
				var propertyMetadatas = ((IEnumerable<OutputFieldMetadata>)fieldMetadata.CustomProperties["Columns"])
					.OrderBy(t => t.OrderIndex);

				foreach (var propertyMetadata in propertyMetadatas)
				{
					var collection = this.columnRenderer.GetColumns(propertyMetadata.Id, propertyMetadata, dataItems);
					columns.AddRange(collection);
				}
			}

			return ExcelGenerator.Generate(property, columns, dataItems);
		}
	}
}