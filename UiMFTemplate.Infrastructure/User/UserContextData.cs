namespace UiMFTemplate.Infrastructure.User
{
	using Newtonsoft.Json;

	public class UserContextData
	{
		[JsonConstructor]
		public UserContextData(
			[JsonProperty(nameof(UserName))] string userName,
			[JsonProperty(nameof(UserId))] string userId)
		{
			this.UserName = userName;
			this.UserId = userId;
		}

		public string UserId { get; }

		public string UserName { get; }
	}
}
