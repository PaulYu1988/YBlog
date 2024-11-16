using Microsoft.EntityFrameworkCore;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services.Extensions;

namespace YBlog.Services.Implement
{
    public class UserService : IUserService
    {
        protected YBlogContext _context;
        public UserService(YBlogContext context)
        {
            _context = context;
        }
        public async Task<bool> RegisterAsync(User user, UserCredential userCredential)
        {
            var result = false;
            _context.Users.Add(user);
            result = await _context.SaveChangesAsync() > 0;
            if (result)
            {
                userCredential.UserId = user.Id;
                _context.UserCredentials.Add(userCredential);
                result = await _context.SaveChangesAsync() > 0;
            }
            return result;
        }

        public Task<List<User>> GetActiveUsersAsync(int take)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> GetAsync(UserPagedQuery query)
        {
            var queryable = _context.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query.Nickname))
            {
                queryable = queryable.Where(x => x.Nickname.Contains(query.Nickname));
            }
            if (query.Type.HasValue)
            {
                queryable = queryable.Where(x => x.Type == query.Type);
            }
            if (query.Status.HasValue)
            {
                queryable = queryable.Where(x => x.Status == query.Status);
            }
            var items = await queryable.OrderByDescending(x => x.Id).ToPagedListAsync(query);
            return items;
        }

        public Task<User?> GetByIdAsync(int id)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> IsUsernameExistAsync(string username)
        {
            var item = await _context.UserCredentials.FirstOrDefaultAsync(x => x.Username == username);
            return item != null ? true : false;
        }

        public async Task<UserCredential?> LoginAsync(LoginRequest request, string token)
        {
            var userCredential = await _context.UserCredentials.FirstOrDefaultAsync(x => x.Username == request.Username && x.Password == request.Password);
            if (userCredential != null)
            {
                if (request.StayLogin)
                {
                    userCredential.Token = token;
                    userCredential.TokenExpirationTime = DateTime.Now.AddDays(30);
                }
                else
                {
                    userCredential.Token = string.Empty;
                    userCredential.TokenExpirationTime = DateTime.Now;
                }
                await _context.SaveChangesAsync();
            }
            return userCredential;
        }

        public async Task<bool> UpdateAsync(UserRequest request)
        {
            var result = false;
            if (request.Id.HasValue)
            {
                var curr = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
                if (curr != null)
                {
                    curr.Introduction = request.Introduction;
                    curr.Avatar = request.Avatar;
                    curr.Email = request.Email;
                    curr.Mobile = request.Mobile;
                    curr.Job = request.Job;
                    curr.Nickname = request.Nickname ?? curr.Id.ToString();
                    if (request.Type.HasValue)
                    {
                        curr.Type = request.Type.Value;
                    }
                    if (request.Status.HasValue)
                    {
                        curr.Status = request.Status.Value;
                    }
                    curr.LastModifiedAt = DateTime.Now;
                    result = (await _context.SaveChangesAsync()) > 0;
                }
            }
            return result;
        }

        public async Task<UserCredential?> GetUserCredentialById(int id)
        {
            return await _context.UserCredentials.FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<bool> ChangePasswordAsync(int id, string password)
        {
            var result = false;
            var curr = await _context.UserCredentials.FirstOrDefaultAsync(x => x.UserId == id);
            if (curr != null)
            {
                curr.Password = password;
                curr.LastModifiedAt = DateTime.Now;
                result = (await _context.SaveChangesAsync()) > 0;
            }
            return result;
        }

        public async Task<User?> UpdateLastLoginedAtAsync(int id)
        {
            var curr = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (curr != null)
            {
                curr.LastLoginedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            return curr;
        }

        public async Task<bool> IsNicknameExistAsync(string nickname, int id)
        {
            var item = await _context.Users.FirstOrDefaultAsync(x => x.Id != id && x.Nickname == nickname);
            return item != null ? true : false;
        }
    }
}
