namespace UiMFTemplate.Infrastructure
{
	using System;
	using System.Collections.Generic;
	using System.Data.Common;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using Humanizer;
	using MediatR;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Data.SqlClient;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Internal;
	using Microsoft.EntityFrameworkCore.Metadata;
	using Microsoft.EntityFrameworkCore.Metadata.Builders;
	using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
	using MySql.Data.MySqlClient;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
	using UiMFTemplate.Infrastructure.Domain;
	using UiMFTemplate.Infrastructure.Emails;
	using UiMFTemplate.Infrastructure.Forms;
	using UiMFTemplate.Infrastructure.Forms.Attributes;
	using UiMFTemplate.Infrastructure.Forms.ClientFunctions;
	using UiMFTemplate.Infrastructure.Forms.Menu;
	using UiMFTemplate.Infrastructure.Forms.Outputs;
	using UiMFTemplate.Infrastructure.Forms.Typeahead;
	using UiMFTemplate.Infrastructure.Security;
	using UiMFTemplate.Infrastructure.User;
	using UiMetadataFramework.Basic.Input;
	using UiMetadataFramework.Basic.Input.Typeahead;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Basic.Response;
	using UiMetadataFramework.Core;
	using UiMetadataFramework.Core.Binding;
	using UiMetadataFramework.MediatR;

	public static class Extensions
	{
		public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> listToAdd)
		{
			foreach (var element in listToAdd)
			{
				list.Add(element);
			}
		}

		public static ActionList AsActionList<T>(this IEnumerable<T> links)
			where T : FormLink
		{
			// ReSharper disable once CoVariantArrayConversion
			return new ActionList(links.Where(t => t != null).ToArray());
		}

		public static HtmlString AsHtmlString(this Enum enumValue)
		{
			var attribute = enumValue.GetAttribute<HtmlStringAttribute>();
			return new HtmlString
			{
				Value = attribute?.Html ?? enumValue.Humanize(),
			};
		}

		public static InlineForm AsInlineForm(this FormLink formLink)
		{
			return new InlineForm
			{
				Form = formLink.Form,
				InputFieldValues = formLink.InputFieldValues
			};
		}

		public static MenuItem AsMenuItem(this FormLink formLink, string menuGroup, int orderIndex = 0)
		{
			return new MenuItem(menuGroup, orderIndex)
			{
				Label = formLink.Label,
				Form = formLink.Form,
				InputFieldValues = formLink.InputFieldValues
			};
		}

		public static MultiSelect<T> AsMultiSelect<T>(this IEnumerable<T> items)
		{
			return new MultiSelect<T>(items.ToArray());
		}

		public static ObjectList<T> AsObjectList<T>(this IEnumerable<T> items, MetadataBinder binder)
		{
			return new ObjectList<T>(items, binder);
		}

		public static RedirectResponse AsRedirectResponse(this FormLink formLink)
		{
			return new RedirectResponse
			{
				Form = formLink.Form,
				InputFieldValues = formLink.InputFieldValues
			};
		}

		public static Tab AsTab(this FormLink formLink, string style = null)
		{
			return new Tab
			{
				Form = formLink.Form,
				InputFieldValues = formLink.InputFieldValues,
				Label = formLink.Label,
				Style = style
			};
		}

		public static FormLink AsText(this FormLink formLink)
		{
			return new FormLink
			{
				Label = formLink.Label
			};
		}

		public static TypeaheadResponse<T> AsTypeaheadResponse<TItem, T>(
			this IList<TItem> queryable,
			Func<TItem, T> value,
			Func<TItem, string> label)
		{
			return new TypeaheadResponse<T>
			{
				Items = queryable.Select(t => new TypeaheadItem<T>
				{
					Label = label(t),
					Value = value(t)
				}),
				TotalItemCount = queryable.Count
			};
		}

		public static string Clean(this string strIn)
		{
			if (strIn == null)
			{
				return null;
			}

			return Regex.Replace(strIn, @"\s+", " ");
		}

		public static string CleanAndLower(this string strIn)
		{
			// Replace multiple spaces with empty strings.
			var cleanSpaces = strIn?.Clean();
			return cleanSpaces?.ToLower();
		}

		public static bool Contains(this string value, string substring, StringComparison comparison)
		{
			return value.IndexOf(substring, comparison) >= 0;
		}

		public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> condition)
		{
			return items
				.GroupBy(condition)
				.Select(t => t.First());
		}

		public static string EnforceMaxLength(this string value, int maxLength)
		{
			if (maxLength < 0)
			{
				throw new ArgumentException("Max length cannot be less than zero.", nameof(maxLength));
			}

			if (value?.Length > maxLength)
			{
				throw new BusinessException($"Maximum allowed length exceeded. At most {maxLength} characters is allowed.");
			}

			return value;
		}

		public static T EnforceNotNull<T>(this T value, string errorMessage = null)
		{
			if (value == null)
			{
				throw new ArgumentException(errorMessage ?? "Value cannot be null.");
			}

			return value;
		}

		public static string EnforceNotNullOrWhiteSpace(this string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				throw new ArgumentException("Value cannot be null.");
			}

			return value;
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

		public static T FindOrException<T>(this DbSet<T> dbSet, object key) where T : class
		{
			var entity = dbSet.Find(key);

			if (entity == null)
			{
				throw new BusinessException("Item not found");
			}

			return entity;
		}

		public static async Task<T> FindOrExceptionAsync<T>(this DbSet<T> dbSet, object key) where T : class
		{
			var entity = await dbSet.FindAsync(key);

			if (entity == null)
			{
				throw new BusinessException("Item not found");
			}

			return entity;
		}

		public static IEnumerable<string> FindPrimaryKeyNames<T>(this DbContext dbContext, T entity)
		{
			return from p in dbContext.FindPrimaryKeyProperties(entity)
				select p.Name;
		}

		public static IEnumerable<object> FindPrimaryKeyValues<T>(this DbContext dbContext, T entity)
		{
			return from p in dbContext.FindPrimaryKeyProperties(entity)
				select entity.GetPropertyValue(p.Name);
		}

		public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (var item in items)
			{
				action(item);
			}
		}

		public static DbConnection GetConnection(this DbContextOptions options)
		{
			if (options.Extensions.FirstOrDefault(t => t is MySqlOptionsExtension) is MySqlOptionsExtension ext)
			{
				return new MySqlConnection(ext.ConnectionString);
			}

			return new SqlConnection(options.FindExtension<SqlServerOptionsExtension>().ConnectionString);
		}

		public static string GetExcelParameters<T>(this IRequest<T> request)
		{
			var properties = request.GetType().GetProperties();
			var parameters = "";
			foreach (var prop in properties)
			{
				object value;
				if (prop.PropertyType == typeof(Paginator))
				{
					value = new Paginator
					{
						PageIndex = 1,
						PageSize = int.MaxValue
					};
				}
				else if (prop.Name.Equals(nameof(ExcelRequest<T>.IsExcelRequest)))
				{
					value = true;
				}
				else
				{
					value = prop.GetValue(request);
				}

				if (value != null)
				{
					var jsonValue = JsonConvert.SerializeObject(value);
					parameters += $"{prop.Name}={jsonValue}&";
				}
			}

			return parameters;
		}

		public static TypeaheadResponse<TKey> GetForTypeahead<TItem, TKey>(
			this IQueryable<TItem> queryable,
			TypeaheadRequest<TKey> request,
			Expression<Func<TItem, TypeaheadItem<TKey>>> select,
			Expression<Func<TItem, bool>> getById,
			Expression<Func<TItem, bool>> getByQuery,
			int maxItemsToRetrieve = TypeaheadRequest<int>.ItemsPerRequest)
		{
			if (request.GetByIds)
			{
				var items = queryable.Where(getById).Select(select).ToList();
				return new TypeaheadResponse<TKey>
				{
					Items = items,
					TotalItemCount = items.Count
				};
			}
			else
			{
				var items = queryable.Where(getByQuery).Select(select);

				return new TypeaheadResponse<TKey>
				{
					Items = items.Take(maxItemsToRetrieve),
					TotalItemCount = items.Count()
				};
			}
		}

		public static string GetInnerExceptionMessage(this Exception exception)
		{
			var message = exception.Message;

			var innerException = exception;

			while (innerException.InnerException != null)
			{
				innerException = innerException.InnerException;
				message = innerException.Message;
			}

			return message;
		}

		public static T GetValueOrDefault<T>(this Dictionary<string, T> dictionary, string key, T defaultValue)
		{
			if (dictionary.TryGetValue(key, out var value))
			{
				return value;
			}

			return defaultValue;
		}

		public static bool In<T>(this T value, params T[] allowed)
		{
			foreach (var a in allowed)
			{
				if (value.Equals(a))
				{
					return true;
				}
			}

			return false;
		}

		public static bool IsImageFilename(this string filename)
		{
			return new[] { "png", "jpg", "gif", "jpeg", "bmp" }.Any(t => filename.EndsWith(t, StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsNot(this string me, params string[] values)
		{
			foreach (var value in values)
			{
				if (me.Equals(value, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}

			return true;
		}

		public static string Join<T>(this IEnumerable<T> items, string separator)
		{
			return string.Join(separator, items);
		}

		public static void RegisterUiMetadata(this DependencyInjectionContainer dependencyInjectionContainer, Assembly assembly)
		{
			dependencyInjectionContainer.GetInstance<ActionRegister>().RegisterAssembly(assembly);
			dependencyInjectionContainer.GetInstance<MetadataBinder>().RegisterAssembly(assembly);
			dependencyInjectionContainer.GetInstance<FormRegister>().RegisterAssembly(assembly);
			dependencyInjectionContainer.GetInstance<MenuRegister>().RegisterAssembly(assembly);
			dependencyInjectionContainer.GetInstance<EntitySecurityConfigurationRegister>().RegisterAssembly(assembly);
			dependencyInjectionContainer.GetInstance<UserRoleCheckerRegister>().RegisterAssembly(assembly);
			dependencyInjectionContainer.GetInstance<EventManager>().RegisterAssembly(assembly);
			dependencyInjectionContainer.GetInstance<ObjectSecurityConfigurationRegister>().RegisterAssembly(assembly);
			dependencyInjectionContainer.GetInstance<EmailTemplateRegister>().RegisterAssembly(assembly);
		}

		public static void Remove<T>(this ICollection<T> list, Func<T, bool> predicate)
		{
			var itemsToRemove = list.Where(predicate).ToList();
			foreach (var itemToRemove in itemsToRemove)
			{
				list.Remove(itemToRemove);
			}
		}

		public static FormLink Secure<T>(this FormLink<T> formLink, UserSecurityContext userSecurityContext)
			where T : class
		{
			if (formLink.ContextId != null)
			{
				return userSecurityContext.CanAccess<T>(formLink.ContextId.Value)
					? formLink
					: formLink.AsText();
			}

			return userSecurityContext.CanAccess<T>()
				? formLink
				: formLink.AsText();
		}

		public static T SingleOrException<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> where = null)
		{
			var item = where != null
				? queryable.SingleOrDefault(where)
				: queryable.SingleOrDefault();

			if (item == null)
			{
				throw new BusinessException("The item is not registered in the system");
			}

			return item;
		}

        public static T SingleOrException<T>(this List<T> queryable, Func<T, bool> where = null)
        {
            var item = where != null
                ? queryable.SingleOrDefault(where)
                : queryable.SingleOrDefault();

            if (item == null)
            {
                throw new BusinessException("The item is not registered in the system");
            }

            return item;
        }

        public static async Task<T> SingleOrExceptionAsync<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> where = null)
		{
			var item = where != null
				? await queryable.SingleOrDefaultAsync(where)
				: await queryable.SingleOrDefaultAsync();

			if (item == null)
			{
				throw new BusinessException("The item is not registered in the system");
			}

			return item;
		}

    public static string SubstringAfterLast(this string value, string search, StringComparison comparisonType)
		{
			if (value == null)
			{
				return null;
			}

			var last = value.LastIndexOf(search, comparisonType);
			if (last >= 0 && last + 1 < value.Length)
			{
				return value.Substring(last + 1);
			}

			return null;
		}

		public static T ToEnum<T>(this string value) where T : struct
		{
			return Enum.TryParse(value, true, out T result) ? result : default(T);
		}

		public static object ToJObject(this IQueryCollection query)
		{
			var parameter = query.ToList();

			var inputValues = new Dictionary<string, object>();
			foreach (var param in parameter)
			{
				inputValues.Add(param.Key, JsonConvert.DeserializeObject(param.Value));
			}

			return JObject.Parse(JsonConvert.SerializeObject(inputValues));
		}

		public static string ToYesOrNoString(this bool boolean)
		{
			return boolean ? "Yes" : "No";
		}

		public static PaginatedData<TResult> Transform<TSource, TResult>(this PaginatedData<TSource> paginatedData,
			Func<TSource, TResult> transform)
		{
			return new PaginatedData<TResult>
			{
				Results = paginatedData.Results.Select(transform).ToList(),
				TotalCount = paginatedData.TotalCount
			};
		}

		public static FormLink WithAction(this FormLink formLink, string action)
		{
			return new FormLink
			{
				Label = formLink.Label,
				Form = formLink.Form,
				InputFieldValues = formLink.InputFieldValues,
				Action = action
			};
		}

		public static MyFormLink WithCustomUi(this FormLink formLink, string cssClass, string confirmationMessage = null, string target = null)
		{
			return new MyFormLink
			{
				Label = formLink.Label,
				Form = formLink.Form,
				InputFieldValues = formLink.InputFieldValues,
				Action = formLink.Action,
				CssClass = cssClass,
				ConfirmationMessage = confirmationMessage,
                Target = target
            };
		}

		public static T WithGrowlMessage<T>(this T response, string message, GrowlNotificationStyle style)
			where T : FormResponse<MyFormResponseMetadata>
		{
            return response.WithGrowlMessage(null, message, style);
		}

		public static T WithGrowlMessage<T>(this T response, string heading, string message, GrowlNotificationStyle style)
			where T : FormResponse<MyFormResponseMetadata>
		{
			response.Metadata = response.Metadata ?? new MyFormResponseMetadata();
			response.Metadata.FunctionsToRun = response.Metadata.FunctionsToRun ?? new List<ClientFunctionMetadata>();

			var growlFunction = new GrowlNotification(heading, message, style).GetClientFunctionMetadata();
			
			response.Metadata.FunctionsToRun.Add(growlFunction);

			return response;
		}

        public static T WithRedirect<T>(this T response, FormLink form)
            where T : FormResponse<MyFormResponseMetadata>
        {
            response.Metadata = response.Metadata ?? new MyFormResponseMetadata();
            response.Metadata.FunctionsToRun = response.Metadata.FunctionsToRun ?? new List<ClientFunctionMetadata>();
            var customProperties = new Dictionary<string, object>()
                .Set("Form", form.Form)
                .Set("InputFieldValues", form.InputFieldValues);
            var clientMetadata = new ClientFunctionMetadata("redirect", customProperties);

            response.Metadata.FunctionsToRun.Add(clientMetadata);

            return response;
        }

		internal static T GetCustomProperty<T>(this FormMetadata form, string propertyName)
		{
			if (form.CustomProperties != null &&
				form.CustomProperties.TryGetValue(propertyName, out var result))
			{
				return (T)result;
			}

			return default(T);
		}

		private static IEnumerable<IProperty> FindPrimaryKeyProperties<T>(this IDbContextDependencies dbContext, T entity)
		{
			return dbContext.Model.FindEntityType(entity.GetType()).FindPrimaryKey().Properties;
		}

		private static object GetPropertyValue<T>(this T entity, string name)
		{
			return entity.GetType().GetProperty(name)?.GetValue(entity, null);
		}

		public static int CalculateAge(this DateTime dob)
		{
			var now = DateTime.Now;
			var years = new DateTime(now.Subtract(dob).Ticks).Year - 1;
			var dobDateNow = dob.AddYears(years);
			var months = 0;
			for (var i = 1; i <= 12; i++)
			{
				if (dobDateNow.AddMonths(i) == now)
				{
					months = i;
					break;
				}
				if (dobDateNow.AddMonths(i) >= now)
				{
					months = i - 1;
					break;
				}
			}
			var days = now.Subtract(dobDateNow.AddMonths(months)).Days;

			if (months >= 6)
			{
				years += 1;
			}

			return years;
		}

		public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				return e.Types.Where(t => t != null);
			}
		}

	}
}
