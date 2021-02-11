namespace UiMFTemplate.Conversations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using UiMetadataFramework.Basic.Output;
	using UiMFTemplate.Conversations.Notification;
	using UiMFTemplate.Notifications;

	internal static class Extensions
	{
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> seenKeys = new HashSet<TKey>();
			foreach (TSource element in source)
			{
				if (seenKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
		}

		public static void EnforceMaxLength(this string value, int maxLength)
		{
			if (maxLength < 0)
			{
				throw new ArgumentException("Max length cannot be less than zero.", nameof(maxLength));
			}

			if (value?.Length > maxLength)
			{
				throw new ArgumentException($"Maximum allowed length exceeded. At most {maxLength} characters is allowed.");
			}
		}

		public static void EnumerableNavigationProperty<T>(
			this EntityTypeBuilder<T> entity,
			string propertyName,
			string fieldName) where T : class
		{
			var childrenProperty = entity.Metadata.FindNavigation(propertyName);
			childrenProperty.SetPropertyAccessMode(PropertyAccessMode.Field);
			childrenProperty.SetField(fieldName);
		}

		public static string GetUrl(this FormLink link)
		{
			return $"{link.Form}?{string.Join("&", link.InputFieldValues.Select(t => t.Key + "=" + t.Value))}";
		}

		public static async Task PublishForUser(this NotificationsDbContext ns,
			int userId,
			string entityType,
			string entityKey,
			string summary,
			string description)
		{
			var notification = new Notifications.Notification(
				new EntityReference(NotificationRecipientType.UserId.Value, userId.ToString()),
				new EntityReference(entityType, entityKey),
				summary,
				description);

			ns.Notifications.Add(notification);
			await ns.SaveChangesAsync();
		}
	}
}
