namespace UiMFTemplate.Excel
{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text.RegularExpressions;
	using ExcelExporter.Core;
	using OfficeOpenXml;
	using UiMFTemplate.Infrastructure;
	using File = Filer.Core.File;

	public static class Extensions
	{
		/// <summary>
		/// Writes <see cref="ExcelFile"/> object to <see cref="HttpResponseMessage"/> 
		/// with "content-disposition=attachment".
		/// </summary>
		/// <param name="excelFile"><see cref="ExcelFile"/> instance.</param>
		/// <param name="filename">Name of the attachment file, without the extension.</param>
		/// <returns><see cref="HttpResponseMessage"/> instance.</returns>
		public static HttpResponseMessage AsHttpResponseMessage(this ExcelFile excelFile, string filename)
		{
			var httpResponseMessage = new HttpResponseMessage
			{
				Content = new ByteArrayContent(excelFile.Data)
				{
					Headers =
					{
						ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = filename + ".xlsx" },
						ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
					}
				}
			};

			return httpResponseMessage;
		}

		public static Dictionary<string, int> CheckWorkSheetColumns(this ExcelWorksheet worksheet, List<string> requiredColumns)
		{
			var columns = new Dictionary<string, int>();
			for (var col = 1; col <= worksheet.Dimension.End.Column; col++)
			{
				var column = worksheet.Cells[1, col].Text;
				if (column != "")
				{
					columns.Add(column.ToLower(), col);
				}
			}

			var missingColumns = requiredColumns.Where(c => !columns.ContainsKey(c.ToLower())).ToList();

			if (missingColumns.Any())
			{
				var missingString = string.Join(",", missingColumns);
				throw new BusinessException($"Column(s) missing from template: {missingString}.");
			}

			return columns;
		}

		public static string GetPlainTextFromHtml(this string htmlString)
		{
			const string HtmlTagPattern = "<.*?>";
			var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			htmlString = regexCss.Replace(htmlString, string.Empty);
			htmlString = Regex.Replace(htmlString, HtmlTagPattern, string.Empty);
			htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
			htmlString = htmlString.Replace("&nbsp;", string.Empty);

			return htmlString;
		}

		public static string GetText(this ExcelWorksheet worksheet, string columnName, int row, Dictionary<string, int> columns)
		{
			if (columns.TryGetValue(columnName.ToLower(), out var column))
			{
				return worksheet.Cells[row, column]?.Text.Trim();
			}

			return null;
		}

		public static object GetValue(this ExcelWorksheet worksheet, string columnName, int row, Dictionary<string, int> columns)
		{
			if (columns.TryGetValue(columnName.ToLower(), out var column))
			{
				return worksheet.Cells[row, column]?.Value;
			}

			return null;
		}

		public static ExcelPackage LoadExcelPackage(this File file)
		{
			string[] excelTypes = { ".xlsx", ".xls" };
			if (!excelTypes.Contains(file.Extension))
			{
				throw new BusinessException("Please enter valid excel file!");
			}

			var stream = new MemoryStream(file.Data.Data);
			var package = new ExcelPackage(stream);
			return package;
		}

		public static List<Column<string>> ToExcelColumns(this List<string> columns)
		{
			return columns.Select(a => new Column<string>(a, null)).ToList();
		}
	}
}