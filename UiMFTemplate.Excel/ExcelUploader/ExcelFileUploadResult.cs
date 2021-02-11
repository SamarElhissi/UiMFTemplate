namespace UiMFTemplate.Excel.ExcelUploader
{
	using System.Collections.Generic;

	public class ExcelFileUploadResult
	{
		public IList<ExcelFileError> Errors { get; set; } = new List<ExcelFileError>();
		public UploadSummary UploadSummary { get; set; } = new UploadSummary();

		public void AddError(string column, int row)
		{
			this.Errors.Add(new ExcelFileError(column, row));
		}

		public void DateTimeFieldError(string column, int row)
		{
			this.Errors.Add(new ExcelFileError(column, row)
			{
				Error = $"«·ﬁÌ„… ›Ì «·⁄„Êœ'{column}' «·’› #{row} ÌÃ» √‰  ﬂÊ‰ „‰ ‰Ê⁄  «—ÌŒ Êﬁ "
			});
		}

		public void NumericFieldError(string column, int row)
		{
			this.Errors.Add(new ExcelFileError(column, row)
			{
				Error = $"«·ﬁÌ„… ›Ì «·⁄„Êœ'{column}' ›Ì ’› #{row} ÌÃ» √‰  ﬂÊ‰ —ﬁ„"
			});
		}

		public void RequiredFieldError(string column, int row)
		{
			this.Errors.Add(new ExcelFileError(column, row)
			{
				Error = $"«·ﬁÌ„… ›Ì ⁄„Êœ'{column}' ›Ì ’› #{row} ÌÃ» √‰ ·«  ﬂÊ‰ ›«—€…"
			});
		}
	}

	public class UploadSummary
	{
		public int? AcceptedRows { get; set; }
		public int? RejectedRows { get; set; }
	}
}
