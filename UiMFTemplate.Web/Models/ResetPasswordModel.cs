namespace UiMFTemplate.Web.Models
{
	public class ResetPasswordModel
	{
		public string Password { get; set; }
		public string Token { get; set; }
		public int UserId { get; set; }
	}
}