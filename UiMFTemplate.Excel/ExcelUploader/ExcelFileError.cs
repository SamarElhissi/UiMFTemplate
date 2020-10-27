namespace UiMFTemplate.Excel.ExcelUploader
{
	public class ExcelFileError
	{
		public ExcelFileError(string column, int row)
		{
			this.Column = column;
			this.Row = row;
			this.Error = $"·„ Ì „ «· ⁄—› ⁄·Ï «·ﬁÌ„… ›Ì '{this.Column}' Ê’› #{this.Row}";
		}

		public string Column { get; set; }
		public string Error { get; set; }
		public int Row { get; set; }
	}
}