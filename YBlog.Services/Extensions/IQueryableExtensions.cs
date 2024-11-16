using YBlog.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace YBlog.Services.Extensions
{
    public static class IQueryableExtensions
    {
        public static async Task<List<T>> ToPagedListAsync<T>(this IQueryable<T> queryable, PagedQuery query)
        {
            var totalCount = await queryable.CountAsync();
            query.TotalCount = totalCount;
            query.PageCount = totalCount % query.PageSize == 0 ? totalCount / query.PageSize : (totalCount / query.PageSize + 1);
            return await queryable.Skip(((query.Page ?? 1) - 1) * query.PageSize).Take(query.PageSize).ToListAsync();
        }
    }
}
