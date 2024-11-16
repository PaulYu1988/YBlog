using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;

namespace YBlog.Services.Mock
{
    public class MockAdvertisementService : IAdvertisementService
    {
        public Task<List<Advertisement>> GetAsync(AdvertisementPagedQuery query)
        {
            return Task.FromResult(GetList());
        }

        public Task<Advertisement?> GetByIdAsync(int id)
        {
            return Task.FromResult(GetList().FirstOrDefault(x => x.Id == id));
        }

        public Task<bool> UpdateAsync(AdvertisementRequest request)
        {
            return Task.FromResult(true);
        }

        private List<Advertisement> GetList()
        {
            var items = new List<Advertisement>();
            items.Add(new Advertisement
            {
                Code = "HtmlHead",
                AdvertisementContent = "<script>alert('HtmlHead');</script>",
                IsEnabled = true,
                CreatedAt = DateTime.Now,
                Id = 1,
                Name = "Hmtl头部",
                LastModifiedAt = DateTime.Now,
            });
            items.Add(new Advertisement
            {
                Code = "NavBottom",
                AdvertisementContent = "<script>alert('NavBottom');</script>",
                IsEnabled = false,
                CreatedAt = DateTime.Now,
                Id = 2,
                Name = "导航下方",
                LastModifiedAt = DateTime.Now,
            });
            return items;
        }
    }
}
