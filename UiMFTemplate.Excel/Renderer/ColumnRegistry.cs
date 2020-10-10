namespace UiMFTemplate.Excel.Renderer
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using UiMFTemplate.Infrastructure;

	public class ColumnRegistry
	{
		private readonly Dictionary<string, ColumnFactory> columns = new Dictionary<string, ColumnFactory>();

		static ColumnRegistry()
		{
			Default = new ColumnRegistry();
			Default.RegisterAssembly(Assembly.GetExecutingAssembly());
		}

		public static ColumnRegistry Default { get; set; }

		public ColumnFactory FactoryFor(string columnType)
		{
			return this.columns.ContainsKey(columnType) ? this.columns[columnType] : null;
		}

		public void RegisterAssembly(Assembly assembly)
		{
			assembly.ExportedTypes
				.Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ColumnFactory)))
				.Select(Activator.CreateInstance)
				.Cast<ColumnFactory>()
				.ForEach(t => this.columns.Add(t.Name, t));
		}

		public void RegisterColumnType(string name, ColumnFactory factory)
		{
			this.columns.Add(name, factory);
		}
	}
}
