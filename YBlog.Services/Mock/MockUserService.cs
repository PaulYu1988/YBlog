using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services.Mock
{
    public class MockUserService : IUserService
    {
        public Task<bool> RegisterAsync(User user, UserCredential userCredential)
        {
            return Task.FromResult(true);
        }

        public Task<List<User>> GetActiveUsersAsync(int take)
        {
            var items = GetMockList(take);
            return Task.FromResult(items);
        }

        public Task<List<User>> GetAsync(UserPagedQuery query)
        {
            var items = GetMockList(query.PageSize);
            query.TotalCount = 20;
            return Task.FromResult(items);
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return Task.FromResult(GetMockList().FirstOrDefault(x => x.Id == id));
        }

        public Task<bool> IsUsernameExistAsync(string username)
        {
            return Task.FromResult(false);
        }

        public Task<UserCredential?> LoginAsync(LoginRequest request, string token)
        {
            UserCredential? result = null;
            if (request?.Username == "paulyu1988")
            {
                result = new UserCredential()
                {
                    Id = 1,
                    Password = "xxxxxx",
                    CreatedAt = DateTime.Now,
                    LastModifiedAt = DateTime.Now,
                    Token = Guid.NewGuid().ToString(),
                    TokenExpirationTime = DateTime.Now.AddDays(30),
                    UserId = 1,
                    Username = "Username"
                };
            }
            return Task.FromResult(result);
        }

        public Task<bool> UpdateAsync(UserRequest request)
        {
            return Task.FromResult(true);
        }
        private List<User> GetMockList(int take = 20)
        {
            var result = new List<User>();
            for (var i = 0; i < take; i++)
            {
                result.Add(new User()
                {
                    Id = i + 1,
                    Avatar = "/assets/images/avatar.jpg",
                    LastLoginedAt = DateTime.Now.AddHours(0 - i),
                    Nickname = "Nickname" + i,
                    Type = (int)EnumUserTypes.Admin,
                    Status = (int)EnumUserStatuses.Enabled,
                    Mobile = "12345678901",
                    CreatedAt = DateTime.Now.AddDays(-1 - i),
                    Email = "xxx@xxx.xxx"
                });
            }
            return result;
        }

        public Task<UserCredential?> GetUserCredentialById(int id)
        {
            var item = new UserCredential()
            {
                Id = id,
            };
            return Task.FromResult<UserCredential?>(item);
        }

        public Task<bool> ChangePasswordAsync(int id, string password)
        {
            return Task.FromResult(true);
        }

        public Task<User?> UpdateLastLoginedAtAsync(int id)
        {
            return Task.FromResult(GetMockList().FirstOrDefault(x => x.Id == id));
        }

        public Task<bool> IsNicknameExistAsync(string nickname, int id)
        {
            if (nickname == "paulyu")
            {
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
