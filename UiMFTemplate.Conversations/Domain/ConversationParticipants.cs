namespace UiMFTemplate.Conversations.Domain
{
	using System.Collections.Generic;
	using System.Linq;

	public class ConversationParticipants
	{
		public List<Participant> Participants;

		public ConversationParticipants(List<Participant> participants)
		{
			this.Participants = participants;
		}
	}

	public class UserParticipant
	{
		public UserParticipant(int id, string username, string email)
		{
			this.Id = id;
			this.Username = username;
			this.Email = email;
		}

		public string Email { get; set; }
		public int Id { get; set; }
		public string Username { get; set; }
	}

	public class Participant
	{
		public Participant(string name, IEnumerable<UserParticipant> users, bool canPostComment = true)
		{
			this.Name = name;
			this.Description = string.Join(", ", users.Select(t => t.Username));
			this.CanPostComment = canPostComment;
			this.Users = users;
		}

		public bool CanPostComment { get; set; }
		public string Description { get; set; }
		public string Name { get; set; }

		public IEnumerable<UserParticipant> Users { get; set; }
	}
}
