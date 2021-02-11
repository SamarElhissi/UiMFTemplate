namespace UiMFTemplate.Core.Extensions
{
	using System;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Threading;
	using System.Threading.Tasks;
	using Microsoft.EntityFrameworkCore;
	using UiMetadataFramework.Basic.Input.Typeahead;
	using UiMFTemplate.Core.Domain;
	using UiMFTemplate.Infrastructure;
	using UiMFTemplate.Infrastructure.Forms.Typeahead;

	public static class Extensions
	{
		public static string GetColor(this string status)
		{
			switch (status)
			{
				case "Placed":
					return "#A2AAB3";
				case "Confirmed":
					return "#FF66FF";
				case "Failed":
					return "#8E0000";
				case "Cancelled":
					return "#EDD926";
				case "OnDelivery":
					return "orange";
				case "Delivered":
					return "#438D20";
				case "CashierCashCollected":
					return "olive";
				case "CashCollected":
					return "Lime";
				case "InUiMFTemplate":
					return "#0066FF";
				case "ToReturn":
					return "Maroon";
				case "UiMFTemplate":
					return "Thistle";
				case "Packed":
					return "Violet";
				case "PickedUp":
					return "DarkSlateGray";
				case "PackingInProgress":
					return "#EDD926";
				default:
					return "#A2AAB3";
			}
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
				var items = queryable.Where(getById).Select(@select).ToList();
				return new TypeaheadResponse<TKey>
				{
					Items = items,
					TotalItemCount = items.Count
				};
			}
			else
			{
				var items = queryable.Where(getByQuery).Select(@select);

				return new TypeaheadResponse<TKey>
				{
					Items = items.Take(maxItemsToRetrieve),
					TotalItemCount = items.Count()
				};
			}
		}

		public static IQueryable<T> NotDeleted<T>(this IQueryable<T> queryable)
			where T : class, IDeletable
		{
			return queryable.Where(a => !a.IsDeleted);
		}

		public static DateTime? ParseDateTime(this string value)
		{
			DateTime? result = null;
			if (DateTime.TryParse(value, out var datetime))
			{
				result = datetime;
			}
			else if (int.TryParse(value, out var date))
			{
				result = DateTime.FromOADate(date);
			}

			return result;
		}

		/// <summary>
		/// Find undeleted entity
		/// </summary>
		/// <param name="queryable">entity to search.</param>
		/// <param name="predicate"></param>
		/// /// <param name="exceptionMessage"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>Matching request logs.</returns>
		public static T SingleNotDeletedOrException<T>(this IQueryable<T> queryable,
			Expression<Func<T, bool>> predicate,
			string exceptionMessage = null)
			where T : class, IDeletable
		{
			var query = queryable.SingleOrDefault(predicate);

			if (query == null)
			{
				throw new BusinessException("Item not found");
			}

			if (query.IsDeleted)
			{
				if (exceptionMessage != null)
				{
					throw new BusinessException(exceptionMessage);
				}

				throw new BusinessException("Item not found");
			}

			return query;
		}

		/// <summary>
		/// Find undeleted entity
		/// </summary>
		/// <param name="queryable">entity to search.</param>
		/// <param name="predicate"></param>
		/// /// <param name="exceptionMessage"></param>
		/// <param name="cancellationToken"></param>
		/// <returns>Matching request logs.</returns>
		public static async Task<T> SingleNotDeletedOrExceptionAsync<T>(this IQueryable<T> queryable,
			Expression<Func<T, bool>> predicate,
			string exceptionMessage = null,
			CancellationToken cancellationToken = default(CancellationToken))
			where T : class, IDeletable
		{
			var query = await queryable.SingleOrDefaultAsync(predicate, cancellationToken);

			if (query == null)
			{
				throw new BusinessException("Item not found");
			}

			if (query.IsDeleted)
			{
				if (exceptionMessage != null)
				{
					throw new BusinessException(exceptionMessage);
				}

				throw new BusinessException("Item not found");
			}

			return query;
		}
	}
}
