namespace UiMFTemplate.Infrastructure.Forms.Outputs
{
	using Newtonsoft.Json;
	using UiMetadataFramework.Core.Binding;

	[OutputFieldType("pie-graph")]
	public class PieGraph
	{
		[JsonProperty(PropertyName = "chartHeader")]
		public Alert ChartHeader { get; set; }

		public PieChartDataItem[] Data { get; set; }
	}

	public class PieChartDataItem
	{
		[JsonProperty(PropertyName = "data")]
		public decimal Data { get; set; }

		[JsonProperty(PropertyName = "label")]
		public string Label { get; set; }
	}
}
