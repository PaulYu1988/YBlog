using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class AdvertisementsController : Controller
    {
        protected IAdvertisementService _advertisementService;
        protected ICacheService _cacheService;
        protected IOperationService _operationService;
        public AdvertisementsController(IAdvertisementService advertisementService, ICacheService cacheService, IOperationService operationService)
        {
            _advertisementService = advertisementService;
            _cacheService = cacheService;
            _operationService = operationService;
        }
        public async Task<IActionResult> IndexAsync(AdvertisementPagedQuery query)
        {
            var items = await _advertisementService.GetAsync(query);
            ViewBag.Query = query;
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> EditAsync(int? id)
        {
            if (!id.HasValue)
                return Redirect("/manage/advertisements");
            var item = await _advertisementService.GetByIdAsync(id.Value);
            if (item == null)
                return Redirect("/manage/advertisements");
            var advertisement = new AdvertisementRequest()
            {
                Id = item.Id,
                AdvertisementContent = item.AdvertisementContent,
                IsEnabled = item.IsEnabled
            };
            return View(advertisement);
        }
        [HttpPost]
        public async Task<IActionResult> EditAsync(AdvertisementRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            if (!request.Id.HasValue)
            {
                return this.BadRequest();
            }
            var userState = Request.GetUserState();
            var result = await _advertisementService.UpdateAsync(request);
            _cacheService.Clear(EnumCacheNames.Advertisements);
            if (result)
            {
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Update, EnumOperationReferenceTypes.Advertisement, request.Id);
                return this.Success(new { request.Id });
            }
            return this.InternalServerError();
        }
    }
}
