using Microsoft.EntityFrameworkCore;
using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Services.Extensions;

namespace YBlog.Services.Implement
{
    public class OperationService : IOperationService
    {
        protected YBlogContext _context;
        public OperationService(YBlogContext context)
        {
            _context = context;
        }
        public Task CreateAsync(int userId, EnumOperationTypes type, EnumOperationReferenceTypes referenceType, int? referenceId = null, string? description = null)
        {
            var item = new Operation()
            {
                UserId = userId,
                Description = description,
                ReferenceId = referenceId,
                ReferenceType = (int)referenceType,
                Type = (int)type,
            };
            _context.Operations.Add(item);
            return _context.SaveChangesAsync();
        }

        public Task<List<Operation>> GetAsync(OperationPagedQuery query)
        {
            var queryable = _context.Operations.AsQueryable();
            if (query.Type.HasValue)
            {
                queryable = queryable.Where(x => x.Type == query.Type);
            }
            if (query.ReferenceType.HasValue)
            {
                queryable = queryable.Where(x => x.ReferenceType == query.ReferenceType);
            }
            var items = queryable.OrderByDescending(x => x.Id).ToPagedListAsync(query);
            return items;
        }

        public async Task<List<OperationView>> GetViewsAsync(List<Operation> items)
        {
            var result = new List<OperationView>();
            if (items.Any())
            {
                var users = new List<User>();
                var categories = new List<Category>();
                var articles = new List<Article>();
                var tags = new List<Tag>();
                var comments = new List<Comment>();
                var navs = new List<Nav>();
                var links = new List<Link>();
                var advertisements = new List<Advertisement>();
                var userIds = items.Select(x => x.UserId).ToList();
                if (items.Any(x => x.ReferenceType == (int)EnumOperationReferenceTypes.User))
                {
                    var ids = items.Where(x => x.ReferenceType == (int)EnumOperationReferenceTypes.User && x.ReferenceId.HasValue).Select(x => x.ReferenceId ?? 0);
                    userIds.AddRange(ids);
                }
                userIds = userIds.Distinct().ToList();
                users = await _context.Users.Where(x => userIds.Contains(x.Id)).ToListAsync();
                if (items.Any(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Category))
                {
                    var ids = items.Where(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Category && x.ReferenceId.HasValue).Select(x => x.ReferenceId);
                    categories = await _context.Categories.Where(x => ids.Contains(x.Id) && x.IsDeleted == false).ToListAsync();
                }
                if (items.Any(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Article))
                {
                    var ids = items.Where(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Article && x.ReferenceId.HasValue).Select(x => x.ReferenceId);
                    articles = await _context.Articles.Where(x => ids.Contains(x.Id) && x.IsDeleted == false).ToListAsync();
                }
                if (items.Any(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Tag))
                {
                    var ids = items.Where(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Tag && x.ReferenceId.HasValue).Select(x => x.ReferenceId);
                    tags = await _context.Tags.Where(x => ids.Contains(x.Id)).ToListAsync();
                }
                if (items.Any(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Comment))
                {
                    var ids = items.Where(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Comment && x.ReferenceId.HasValue).Select(x => x.ReferenceId);
                    comments = await _context.Comments.Where(x => ids.Contains(x.Id) && x.IsDeleted == false).ToListAsync();
                }
                if (items.Any(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Nav))
                {
                    var ids = items.Where(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Nav && x.ReferenceId.HasValue).Select(x => x.ReferenceId);
                    navs = await _context.Navs.Where(x => ids.Contains(x.Id)).ToListAsync();
                }
                if (items.Any(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Link))
                {
                    var ids = items.Where(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Link && x.ReferenceId.HasValue).Select(x => x.ReferenceId);
                    links = await _context.Links.Where(x => ids.Contains(x.Id)).ToListAsync();
                }
                if (items.Any(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Advertisement))
                {
                    var ids = items.Where(x => x.ReferenceType == (int)EnumOperationReferenceTypes.Advertisement && x.ReferenceId.HasValue).Select(x => x.ReferenceId);
                    advertisements = await _context.Advertisements.Where(x => ids.Contains(x.Id)).ToListAsync();
                }
                foreach (var item in items)
                {
                    var model = new OperationView()
                    {
                        Id = item.Id,
                        UserId = item.UserId,
                        Description = item.Description,
                        CreatedAt = item.CreatedAt,
                        ReferenceId = item.ReferenceId,
                        ReferenceType = item.ReferenceType,
                        Type = item.Type,
                        User = users.FirstOrDefault(x => x.Id == item.UserId)
                    };
                    if (item.ReferenceId.HasValue)
                    {
                        switch (item.ReferenceType)
                        {
                            case (int)EnumOperationReferenceTypes.User:
                                model.Reference = users.FirstOrDefault(x => x.Id == item.ReferenceId);
                                break;
                            case (int)EnumOperationReferenceTypes.Category:
                                model.Reference = categories.FirstOrDefault(x => x.Id == item.ReferenceId);
                                break;
                            case (int)EnumOperationReferenceTypes.Article:
                                model.Reference = articles.FirstOrDefault(x => x.Id == item.ReferenceId);
                                break;
                            case (int)EnumOperationReferenceTypes.Tag:
                                model.Reference = tags.FirstOrDefault(x => x.Id == item.ReferenceId);
                                break;
                            case (int)EnumOperationReferenceTypes.Comment:
                                model.Reference = comments.FirstOrDefault(x => x.Id == item.ReferenceId);
                                break;
                            case (int)EnumOperationReferenceTypes.Nav:
                                model.Reference = navs.FirstOrDefault(x => x.Id == item.ReferenceId);
                                break;
                            case (int)EnumOperationReferenceTypes.Link:
                                model.Reference = links.FirstOrDefault(x => x.Id == item.ReferenceId);
                                break;
                            case (int)EnumOperationReferenceTypes.Advertisement:
                                model.Reference = advertisements.FirstOrDefault(x => x.Id == item.ReferenceId);
                                break;
                        }
                    }
                    result.Add(model);
                }
            }
            return result;
        }
    }
}
