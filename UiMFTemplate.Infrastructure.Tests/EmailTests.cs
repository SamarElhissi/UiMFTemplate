namespace UiMFTemplate.Infrastructure.Tests
{
	using System.Threading.Tasks;
	using FluentAssertions;
	using UiMFTemplate.App.EventNotification.Emails.Templates;
	using UiMFTemplate.Core.Domain;
	using UiMFTemplate.DataSeed;
	using UiMFTemplate.Infrastructure.Configuration;
	using UiMFTemplate.Infrastructure.Emails;
	using Xunit;
	using Task = System.Threading.Tasks.Task;

	public class EmailTests
	{
		[Fact]
		public async Task CanCompileEmail()
		{
			var container = new DataSeedDiContainer();
			var emailTemplateRegister = container.Container.GetInstance<EmailTemplateRegister>();

			var user = RegisteredUser.Create(1, "jack");
			var workItem = new WorkItem("do magic", user);

			var model = new CategoryCreatedTemplate.Model(workItem, new AppConfig());

			var email = await emailTemplateRegister.CompileEmail(
				"test@example.com",
				model);

			email.Should().NotBeNull();
		}
	}
}
