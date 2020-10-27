namespace UiMFTemplate.Excel.Renderer.Factories
{
	using System.Collections.Generic;
	using ExcelExporter.Core;
	using UiMetadataFramework.Core;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.Infrastructure.Forms.Outputs;

	public class LinkColumnFactory : ColumnFactory
	{
		public LinkColumnFactory() : base("link")
		{
		}

		public override IEnumerable<Column<object>> GetColumn(string field, OutputFieldMetadata outputFieldMetadata, IList<object> data)
		{
			yield return new Column<object>(
				outputFieldMetadata.Label,
				row =>
				{
					var value = row.GetPropertyValue(field);

					if (!(value is Link link))
					{
						return new CellData(null);
					}

					var siteRoot = ConfigurationReader.GetConfigurations().GetSection("AppConfig")["SiteRoot"];
					var url = siteRoot + link.Url;
					return new CellData(link.Anchor, url);
				});
		}
	}
}