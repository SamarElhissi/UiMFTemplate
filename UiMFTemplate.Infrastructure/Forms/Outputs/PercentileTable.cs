namespace UiMFTemplate.Infrastructure.Forms.Outputs
{
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core.Binding;

	[OutputFieldType("percentile-table")]
	public class PercentileTable
	{
		public KeyValuePair<PercentileTableData, decimal?>[] Data { get; set; }
	}

	public class PercentileTableData
	{
		public PercentileTableData(decimal age, decimal ravenScore, ActionList actions)
		{
			this.Age = age;
			this.RavenScore = ravenScore;
			this.Actions = actions;
		}

		[JsonProperty(PropertyName = "actions")]
		public ActionList Actions { get; set; }

		[JsonProperty(PropertyName = "age")]
		public decimal Age { get; set; }

		[JsonProperty(PropertyName = "raven")]
		public decimal RavenScore { get; set; }
	}
}