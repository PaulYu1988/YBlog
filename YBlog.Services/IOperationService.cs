using YBlog.Models.Custom;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;

namespace YBlog.Services
{
    public interface IOperationService
    {
        Task<List<Operation>> GetAsync(OperationPagedQuery query);
        Task CreateAsync(int userId, EnumOperationTypes type, EnumOperationReferenceTypes referenceType, int? referenceId = null, string? description = null);
        Task<List<OperationView>> GetViewsAsync(List<Operation> items);
    }
}
