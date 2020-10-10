namespace UiMFTemplate.Infrastructure.Tests
{
	using FluentAssertions;
	using UiMFTemplate.DataSeed;
	using Xunit;

	public class MetadataTest
	{
		[Fact]
		public void MetadataIsConfiguredCorrectly()
		{
			new DataSeedDiContainer().Should().NotBeNull();
		}
	}
}
