namespace UiMFTemplate.Core.Notification
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using UiMFTemplate.Core.Security;
    using UiMFTemplate.Help;
    using UiMFTemplate.Infrastructure;
    using UiMFTemplate.Infrastructure.EntityFramework;
    using UiMFTemplate.Infrastructure.Forms;
    using UiMFTemplate.Infrastructure.Forms.CustomProperties;
    using UiMFTemplate.Infrastructure.Forms.Outputs;
    using UiMFTemplate.Infrastructure.Security;
    using UiMFTemplate.Infrastructure.User;
    using UiMFTemplate.Notifications;
    using UiMetadataFramework.Basic.Input;
    using UiMetadataFramework.Basic.Input.Dropdown;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core;
    using UiMetadataFramework.Core.Binding;

    [MyForm(Id = "my-notifications", PostOnLoad = true, Label = "Notifications", SubmitButtonLabel = UiFormConstants.SearchLabel)]
    [Secure(typeof(CoreActions), nameof(CoreActions.ViewNotifications))]
    [Documentation(DocumentationPlacement.Inline, DocumentationSourceType.String, "Display all notifications")]
    [CssClass(UiFormConstants.InputsVerticalMultipleColumn)]
    public class MyNotifications : MyForm<MyNotifications.Request, MyNotifications.Response>
    {
        public enum NotificationStatus
        {
            [Description("Show only unArchive")]
            ShowNotArchivedOnly = 1,
            [Description("Show only archive")]
            ShowArchivedOnly = 2
        }

        private readonly NotificationManagerRegister notificationManagerRegister;
        private readonly NotificationsDbContext notificationsDbContext;
        private readonly UserContext userContext;

        public MyNotifications(
            UserContext userContext,
            NotificationsDbContext notificationsDbContext,
            NotificationManagerRegister notificationManagerRegister)
        {
            this.userContext = userContext;
            this.notificationsDbContext = notificationsDbContext;
            this.notificationManagerRegister = notificationManagerRegister;
        }

        public static FormLink Button(string label)
        {
            return new FormLink
            {
                Label = label,
                Form = typeof(MyNotifications).GetFormId()
            };
        }

        protected override Response Handle(Request message)
        {
            var query = this.notificationsDbContext
                .Notifications
                .ForUser(this.userContext)
                .Include(s => s.RelatedTo)
                .AsNoTracking();

            if (!string.IsNullOrEmpty(message.Title))
            {
                query = query.Where(a => a.Description.Contains(message.Title) || a.Summary.Contains(message.Title));
            }

            if (message.IsArchived != null)
            {
                if (message.IsArchived.Value == NotificationStatus.ShowArchivedOnly)
                {
                    query = query.Where(t => t.ArchivedOn.HasValue);
                }
                else if (message.IsArchived.Value == NotificationStatus.ShowNotArchivedOnly)
                {
                    query = query.Where(t => !t.ArchivedOn.HasValue);
                }
            }

            var notifications = query
                .OrderByDescending(t => t.Id)
                .Paginate(message.NotificationPaginator);

            return new Response
            {
                Metadata = new MyFormResponseMetadata
                {
                    Title = "Notifications"
                },
                Items = notifications.Transform(t => new NotificationRow
                {
                    //Description = t.Description,
                    CreatedOn = t.CreatedOn,
                    Summary = new HtmlString(t.Summary),
                    Archived = t.ArchivedOn != null ? "Yes" : "No",
                    Link = this.notificationManagerRegister.GetInstance(t.RelatedTo.EntityType).GetLink(t.RelatedTo.EntityId).Link,
                    Actions = GetActions(t, this.notificationManagerRegister.GetInstance(t.RelatedTo.EntityType).GetLink(t.RelatedTo.EntityId).Actions)
                })
            };
        }

        private static ActionList GetActions(Notification t, ActionList actions)
        {
            if (actions != null)
            {
                actions.Actions.Add(t.ArchivedOn != null ? UnArchive.Button(t.Id) : Archive.Button(t.Id));
                return actions;
            }
            return t.ArchivedOn != null
                ? new ActionList(UnArchive.Button(t.Id))
                : new ActionList(Archive.Button(t.Id));
        }

        public class Request : IRequest<Response>
        {
            [InputField(OrderIndex = 10, Label = "Is archive")]
            public DropdownValue<NotificationStatus> IsArchived { get; set; }

            public Paginator NotificationPaginator { get; set; }

            [InputField(Label = "Search")]
            public string Title { get; set; }
        }

        public class Response : FormResponse<MyFormResponseMetadata>
        {
            [OutputField(OrderIndex = 0)]
            public ActionList Actions { get; set; }

            [PaginatedData(nameof(Request.NotificationPaginator), Label = "")]
            public PaginatedData<NotificationRow> Items { get; set; }
        }

        public class NotificationRow
        {
            [OutputField(OrderIndex = 30, Label = "")]
            public ActionList Actions { get; set; }

            [OutputField(OrderIndex = 5)]
            public string Archived { get; set; }

            [OutputField(OrderIndex = 1, Label = "Created on")]
            public DateTime CreatedOn { get; set; }

            [OutputField(OrderIndex = 4)]
            public FormLink Link { get; set; }

            [OutputField(OrderIndex = 3)]
            public HtmlString Summary { get; set; }
        }
    }
}
