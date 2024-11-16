using Microsoft.AspNetCore.Mvc;
using YBlog.Models.Database;
using YBlog.Models.Queries;
using YBlog.Models.Requests;
using YBlog.Services;
using YBlog.Extensions;
using YBlog.Attributes;
using YBlog.Models.Enums;

namespace YBlog.Areas.Manage.Controllers
{
    [Area("Manage")]
    [ManagePostLogin]
    public class CategoriesController : Controller
    {
        protected ICategoryService _categoryService;
        protected IOperationService _operationService;
        public CategoriesController(ICategoryService categoryService, IOperationService operationService)
        {
            _categoryService = categoryService;
            _operationService = operationService;
        }
        public async Task<IActionResult> IndexAsync(CategoryPagedQuery query)
        {
            var items = await _categoryService.GetAsync(query);
            ViewBag.Query = query;
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> EditAsync(int? id)
        {
            var category = new CategoryRequest();
            if (id.HasValue)
            {
                var item = await _categoryService.GetByIdAsync(id.Value);
                if (item != null)
                {
                    category.Id = item.Id;
                    category.Sort = item.Sort;
                    category.Name = item.Name;
                    category.Description = item.Description;
                }
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(CategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }
            var userState = Request.GetUserState();
            var result = false;
            if (request.Id.HasValue)
            {
                result = await _categoryService.UpdateAsync(request);
                if (result)
                {
                    await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Update, EnumOperationReferenceTypes.Category, request.Id, request.Name);
                }
            }
            else
            {
                var category = new Category()
                {
                    Name = request.Name ?? string.Empty,
                    Description = request.Description,
                    IsDeleted = false,
                    Sort = request.Sort
                };
                result = await _categoryService.CreateAsync(category);
                if (result)
                {
                    request.Id = category.Id;
                    await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Create, EnumOperationReferenceTypes.Category, request.Id, request.Name);
                }
            }
            if (!result)
            {
                return this.InternalServerError();
            }
            return this.Success(new
            {
                request.Id
            });
        }

        [HttpDelete]
        public async Task<IActionResult> IndexAsync(int id)
        {
            var curr = await _categoryService.GetByIdAsync(id);
            if (curr == null)
            {
                return this.NotFound();
            }
            var result = await _categoryService.DeleteByIdAsync(id);
            if (result)
            {
                var userState = Request.GetUserState();
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Delete, EnumOperationReferenceTypes.Category, id, curr.Name);
                return this.Success();
            }
            return this.InternalServerError();
        }
    }
}
