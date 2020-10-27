namespace UiMFTemplate.Infrastructure.Forms.Attributes
{
	using System;

	public class ScoreAttribute : Attribute
	{
		public ScoreAttribute(int highScore, int stageScore)
		{
			this.HighScore = highScore;
			this.StageScore = stageScore;
		}

		public int HighScore { get; set; }
		public int StageScore { get; set; }
	}
}