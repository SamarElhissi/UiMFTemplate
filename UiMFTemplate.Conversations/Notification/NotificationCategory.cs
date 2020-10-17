namespace UiMFTemplate.Conversations.Notification
{
    using System.Collections.Generic;
    using System.Linq;

    public class NotificationCategory
    {
        private const int ActionableTypeIdIncrement = 1000;
        private static readonly List<NotificationCategory> List;
        public static NotificationCategory Grant = new NotificationCategory(1, "Grant", "Grant");
        public static NotificationCategory Component = new NotificationCategory(2, "Component", "Component");

        static NotificationCategory()
        {
            List = typeof(NotificationCategory)
                .GetFields()
                .Where(a => a.FieldType == typeof(NotificationCategory))
                .Select(f => f.GetValue(null) as NotificationCategory)
                .ToList();
        }

        private NotificationCategory(int id, string name, string tag)
        {
            this.Id = id;
            this.Name = name;
            this.Tag = tag;
            this.Type = CategoryType.Informative;
        }

        private NotificationCategory(NotificationCategory original, CategoryType type)
        {
            if (type == CategoryType.Actionable && original.Type != CategoryType.Actionable)
            {
                this.Id = original.Id + ActionableTypeIdIncrement;
            }

            this.Name = original.Name;
            this.Tag = original.Tag;
            this.Type = type;
        }

        public int Id { get; }
        public string Name { get; }
        public string Tag { get; }
        public CategoryType Type { get; }

        public static NotificationCategory Parse(int? category)
        {
            var normalizedCategory = category;
            if (category >= ActionableTypeIdIncrement)
            {
                normalizedCategory -= ActionableTypeIdIncrement;
                var notificationCategory = List.SingleOrDefault(t => t.Id == normalizedCategory);
                return notificationCategory?.AsActionable();
            }

            return List.SingleOrDefault(t => t.Id == normalizedCategory);
        }

        public NotificationCategory AsActionable()
        {
            return new NotificationCategory(this, CategoryType.Actionable);
        }
    }
}
