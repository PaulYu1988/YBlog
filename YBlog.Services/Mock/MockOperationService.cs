using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;

namespace YBlog.Services.Mock
{
    public class MockOperationService : IOperationService
    {
        public Task CreateAsync(int userId, EnumOperationTypes type, EnumOperationReferenceTypes referenceType, int? referenceId = null, string? description = null)
        {
            return Task.CompletedTask;
        }

        public Task<List<Operation>> GetAsync(OperationPagedQuery query)
        {
            return Task.FromResult(GetList());
        }

        public Task<List<OperationView>> GetViewsAsync(List<Operation> items)
        {
            return Task.FromResult(GetViewList());
        }

        private List<Operation> GetList()
        {
            var items = new List<Operation>();
            items.Add(new Operation
            {
                Id = 1,
                UserId = 1,
                CreatedAt = DateTime.Now,
                LastModifiedAt = DateTime.Now,
                Type = 1,
                ReferenceType = 1
            });
            items.Add(new Operation
            {
                Id = 1,
                UserId = 1,
                CreatedAt = DateTime.Now,
                LastModifiedAt = DateTime.Now,
                Type = 2,
                ReferenceType = 1,
                ReferenceId = 1
            });
            items.Add(new Operation
            {
                Id = 2,
                UserId = 1,
                CreatedAt = DateTime.Now,
                LastModifiedAt = DateTime.Now,
                Type = 2,
                ReferenceType = 2,
                ReferenceId = 1
            });
            items.Add(new Operation
            {
                Id = 3,
                UserId = 1,
                CreatedAt = DateTime.Now,
                LastModifiedAt = DateTime.Now,
                Type = 3,
                ReferenceType = 3,
                ReferenceId = 1
            });
            items.Add(new Operation
            {
                Id = 4,
                UserId = 1,
                CreatedAt = DateTime.Now,
                LastModifiedAt = DateTime.Now,
                Type = 4,
                ReferenceType = 2,
                ReferenceId = 1
            });
            return items;
        }

        private List<OperationView> GetViewList()
        {
            var items = new List<OperationView>();
            var user = new User()
            {
                Id = 1,
                Nickname = "PaulYu"
            };
            items.Add(new OperationView
            {
                Id = 1,
                UserId = 1,
                CreatedAt = DateTime.Now,
                Type = 1,
                ReferenceType = 1,
                User = user,
                Description = "IP：123.432.231.11"
            });
            items.Add(new OperationView
            {
                Id = 2,
                UserId = 1,
                CreatedAt = DateTime.Now,
                Type = 2,
                ReferenceType = 1,
                User = user,
                Reference = user
            });
            items.Add(new OperationView
            {
                Id = 3,
                UserId = 1,
                CreatedAt = DateTime.Now,
                Type = 2,
                ReferenceType = 2,
                ReferenceId = 1,
                Reference = new Category()
                {
                    Name = "Category name",
                    Id = 1
                },
                User = user
            });
            items.Add(new OperationView
            {
                Id = 4,
                UserId = 1,
                CreatedAt = DateTime.Now,
                Type = 3,
                ReferenceType = 3,
                ReferenceId = 1,
                Reference = new Article()
                {
                    Title = "生活的美好生活的美好",
                    Id = 1,
                    Thumbnail = "xxx"
                },
                User = user
            });
            items.Add(new OperationView
            {
                Id = 5,
                UserId = 1,
                CreatedAt = DateTime.Now,
                Type = 4,
                ReferenceType = 4,
                ReferenceId = 1,
                User = user
            });
            items.Add(new OperationView
            {
                Id = 6,
                UserId = 1,
                CreatedAt = DateTime.Now,
                Type = 3,
                ReferenceType = 9,
                User = user
            });
            items.Add(new OperationView
            {
                Id = 7,
                UserId = 1,
                CreatedAt = DateTime.Now,
                Type = 3,
                ReferenceType = 10,
                User = user
            });
            items.Add(new OperationView
            {
                Id = 8,
                UserId = 1,
                CreatedAt = DateTime.Now,
                Type = 3,
                ReferenceType = 10,
                User = user
            });
            items.Add(new OperationView
            {
                Id = 9,
                UserId = 1,
                CreatedAt = DateTime.Now,
                Type = 3,
                ReferenceType = 10,
                User = user
            });
            items.Add(new OperationView
            {
                Id = 10,
                UserId = 1,
                CreatedAt = DateTime.Now,
                Type = 3,
                ReferenceType = 10,
                User = user
            });
            items.Add(new OperationView { 
                Id = 11,
                UserId = 1,
                CreatedAt = DateTime.Now,
                Type = 4,
                ReferenceType = 11,
                User = user,
                Description = "xxx.jpg"
            });
            return items;
        }
    }
}
