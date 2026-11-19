using Microsoft.AspNetCore.Mvc;
using YBlog.Attributes;
using YBlog.Extensions;
using YBlog.Models.Database;
using YBlog.Models.Enums;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class BotsController : Controller
    {
        protected IBotService _botService;
        protected IOperationService _operationService;

        public BotsController(IBotService botService, IOperationService operationService)
        {
            _botService = botService;
            _operationService = operationService;
        }

        public async Task<IActionResult> IndexAsync(BotPagedQuery query)
        {
            var items = await _botService.GetAsync(query);
            ViewBag.Query = query;
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> EditAsync(int? id)
        {
            var request = new BotRequest();
            if (id.HasValue)
            {
                var item = await _botService.GetByIdAsync(id.Value);
                if (item == null)
                {
                    return Redirect("/manage/bots");
                }
                request.Id = item.Id;
                request.Name = item.Name;
                request.IdentificationCode = item.IdentificationCode;
                request.Type = item.Type;
            }
            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(BotRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var userState = Request.GetUserState();
            bool result;
            if (request.Id.HasValue)
            {
                result = await _botService.UpdateAsync(request);
                if (result)
                {
                    await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Update, EnumOperationReferenceTypes.Bot, request.Id, request.Name);
                }
            }
            else
            {
                var item = new Bot
                {
                    Name = request.Name!,
                    IdentificationCode = request.IdentificationCode!,
                    Type = request.Type!.Value
                };
                result = await _botService.CreateAsync(item);
                if (result)
                {
                    request.Id = item.Id;
                    await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Create, EnumOperationReferenceTypes.Bot, request.Id, request.Name);
                }
            }
            return result ? this.Success(new { request.Id }) : this.InternalServerError();
        }

        [HttpDelete]
        public async Task<IActionResult> IndexAsync(int id)
        {
            var item = await _botService.GetByIdAsync(id);
            if (item == null)
            {
                return this.NotFound();
            }
            var result = await _botService.DeleteByIdAsync(id);
            if (result)
            {
                var userState = Request.GetUserState();
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Delete, EnumOperationReferenceTypes.Bot, id, item.Name);
                return this.Success();
            }
            return this.InternalServerError();
        }
    }
}
