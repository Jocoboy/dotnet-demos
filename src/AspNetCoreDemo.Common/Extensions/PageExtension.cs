using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common.Extensions
{
    public static class PageExtension
    {
        public const int FirstPage = 1;
        public const int DefaultPageSize = 10;
        public static IEnumerable<T> Page<T>(this IEnumerable<T> enumerableItems, int pageSize, int? pageNumber = null)
        {
            return PageFromQuery(enumerableItems.AsQueryable(), pageSize, pageNumber);
        }

        public static IEnumerable<TResult> Page<TSource, TResult>(this IEnumerable<TSource> enumerableItems, Func<TSource, TResult> valueSelector, int pageSize, int? pageNumber = null)
        {
            return PageFromQuery(enumerableItems.AsQueryable(), pageSize, pageNumber).Select(valueSelector);
        }

        public static IEnumerable<TResult> Page<TSource, TResult>(this IQueryable<TSource> queryableItems, Func<TSource, TResult> valueSelector, int pageSize, int? pageNumber = null)
        {
            return PageFromQuery(queryableItems, pageSize, pageNumber).Select(valueSelector);
        }

        public static IEnumerable<TResult> Page<TSource, TResult>(this IEnumerable<TSource> enumerableItems, Expression<Func<TSource, TResult>> valueSelector, int pageSize, int? pageNumber = null)
        {
            return PageFromQuery(enumerableItems.AsQueryable(), pageSize, pageNumber).Select(valueSelector);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> queryableItems, int pageSize, int? pageNumber = null)
        {
            return PageFromQuery(queryableItems, pageSize, pageNumber);
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> queryableItems, int pageSize, int? pageNumber, out int totalNumber)
        {
            return PageFromQuery(queryableItems, pageSize, pageNumber, out totalNumber);
        }

        public static IQueryable<TResult> Page<TSource, TResult>(this IQueryable<TSource> queryableItems, Expression<Func<TSource, TResult>> valueSelector, int pageSize, int? pageNumber = null)
        {
            return PageFromQuery(queryableItems, pageSize, pageNumber).Select(valueSelector);
        }

        private static IQueryable<TSource> PageFromQuery<TSource>(IQueryable<TSource> queryableItems, int pageSize, int? pageNumber)
        {
            var count = queryableItems.Count();
            var isValidCurrentPage = pageNumber != null && pageNumber.Value >= FirstPage;
            var actualPage = isValidCurrentPage ? pageNumber.Value : FirstPage;
            pageSize = pageSize <= 0 ? DefaultPageSize : pageSize;
            var basePage = count / pageSize;
            var allPage = count % pageSize == 0 ? basePage : basePage + 1;
            if (actualPage > allPage && allPage > 0)
            {
                actualPage = allPage;
            }
            var skip = (actualPage - 1) * pageSize;
            return queryableItems.Skip(skip).Take(pageSize);
        }

        private static IQueryable<TSource> PageFromQuery<TSource>(IQueryable<TSource> queryableItems, int pageSize, int? pageNumber, out int count)
        {
            count = queryableItems.Count();
            var isValidCurrentPage = pageNumber != null && pageNumber.Value >= FirstPage;
            var actualPage = isValidCurrentPage ? pageNumber.Value : FirstPage;
            pageSize = pageSize <= 0 ? DefaultPageSize : pageSize;
            var basePage = count / pageSize;
            var allPage = count % pageSize == 0 ? basePage : basePage + 1;
            if (actualPage > allPage && allPage > 0)
            {
                actualPage = allPage;
            }
            var skip = (actualPage - 1) * pageSize;
            return queryableItems.Skip(skip).Take(pageSize);
        }
    }
}
