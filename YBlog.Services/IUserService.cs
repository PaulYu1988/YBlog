using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services
{
    public interface IUserService
    {
        Task<UserCredential?> LoginAsync(LoginRequest request, string token);
        Task<bool> IsUsernameExistAsync(string username);
        Task<bool> IsNicknameExistAsync(string nickname, int id);
        Task<bool> RegisterAsync(User user, UserCredential userCredential);
        Task<bool> UpdateAsync(UserRequest request);
        Task<User?> GetByIdAsync(int id);
        Task<UserCredential?> GetUserCredentialByUserId(int id);
        Task<List<User>> GetAsync(UserPagedQuery query);
        Task<bool> ChangePasswordAsync(int id, string password);
        Task<User?> UpdateLastLoginedAtAsync(int id);
        Task<List<UserCredential>> GetUserCredentialsByUserIds(int[] ids);
    }
}
