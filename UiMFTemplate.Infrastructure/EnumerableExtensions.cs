namespace UiMFTemplate.Infrastructure
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using UiMFTemplate.Infrastructure.Forms.Attributes;

	public static class EnumerableExtensions
	{
		public static TAttribute GetAttribute<TAttribute>(this Enum value)
			where TAttribute : Attribute
		{
			var type = value.GetType();
			var name = Enum.GetName(type, value);

			if (string.IsNullOrWhiteSpace(name))
			{
				return null;
			}

			return type.GetField(name) // I prefer to get attributes this way
				.GetCustomAttributes(false)
				.OfType<TAttribute>()
				.SingleOrDefault();
		}

		public static string GetDescription(this Enum value)
		{
			var attribute = value.GetAttribute<DescriptionAttribute>();
			return attribute.Description;
		}
    }
}
