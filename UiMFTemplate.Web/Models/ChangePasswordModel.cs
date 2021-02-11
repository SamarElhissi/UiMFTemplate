namespace UiMFTemplate.Web.Models
{
	public class ChangePasswordModel
	{
		public string NewPassword { get; set; }
		public string OldPassword { get; set; }
		public int UserId { get; set; }
	}
}
