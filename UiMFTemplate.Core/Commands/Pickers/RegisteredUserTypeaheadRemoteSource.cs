namespace UiMFTemplate.Core.Commands.Pickers
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using UiMFTemplate.Core.DataAccess;
    using UiMFTemplate.Core.Security;
    using UiMFTemplate.Infrastructure;
    using UiMFTemplate.Infrastructure.Forms;
    using UiMFTemplate.Infrastructure.Forms.Typeahead;
    using UiMFTemplate.Infrastructure.Security;
    using UiMFTemplate.Users.Security;
    using UiMetadataFramework.Basic.Input.Typeahead;
    using UiMetadataFramework.Core.Binding;

    [Form]
    [Secure(typeof(UserActions), nameof(UserActions.Login))]
    public class RegisteredUserTypeaheadRemoteSource : TypeaheadRemoteSource<RegisteredUserTypeaheadRemoteSource.Request, int>
    {
        private readonly CoreDbContext context;

        public RegisteredUserTypeaheadRemoteSource(CoreDbContext context)
        {
            this.context = context;
        }

        protected override TypeaheadResponse<int> Handle(Request message)
        {
            var persons = message.GetByIds
                ? this.context.Users.Where(t => message.Ids.Items.Contains(t.Id))
                : this.context.Users.Where(t => t.Id.ToString() == message.Query || t.Name.Contains(message.Query, StringComparison.CurrentCultureIgnoreCase));

            return new TypeaheadResponse<int>
            {
                Items = persons
                    .AsNoTracking()
                    .Take(Request.ItemsPerRequest)
                    .ToList()
                    .Select(t => new TypeaheadItem<int>
                    {
                        Label = t.Name,
                        Value = t.Id
                    }).ToList(),
                TotalItemCount = persons.Count()
            };
        }

        public class Request : TypeaheadRequest<int>
        {
        }

        public class Response
        {
        }
    }
}
