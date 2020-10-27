namespace UiMFTemplate.Excel.Renderer.Factories
{
	using System.Collections.Generic;
	using System.Linq;
	using ExcelExporter.Core;
	using Microsoft.EntityFrameworkCore.Internal;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;
	using UiMFTemplate.Infrastructure.Configuration;

	public class FormLinkColumnFactory : ColumnFactory
	{
		public FormLinkColumnFactory() : base("formlink")
		{
		}

		public override IEnumerable<Column<object>> GetColumn(string field, OutputFieldMetadata outputFieldMetadata, IList<object> data)
		{
			yield return new Column<object>(
				outputFieldMetadata.Label,
				row =>
				{
					var value = row.GetPropertyValue(field);

					if (!(value is FormLink formLink))
					{
						return new CellData(null);
					}

					var siteRoot = ConfigurationReader.GetConfigurations().GetSection("AppConfig")["SiteRoot"];
					if (string.IsNullOrEmpty(formLink.Form))
					{
						return new CellData(formLink.Label);
					}

					var query = formLink.InputFieldValues.Select(t => t.Key + "=" + t.Value).Join("&");
					var url = siteRoot + "#/form/" + formLink.Form + "?" + query;
					return new CellData(formLink.Label, url);
				});
		}
	}
}