﻿using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System.Text.RegularExpressions;
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
    public class ArticlesController : Controller
    {
        protected IWebHostEnvironment _env;
        protected IArticleService _articleService;
        protected ICategoryService _categoryService;
        protected ITagService _tagService;
        protected ICacheService _cacheService;
        protected IOperationService _operationService;
        public ArticlesController(IWebHostEnvironment env, IArticleService articleService, ICategoryService categoryService, ITagService tagService, ICacheService cacheService, IOperationService operationService)
        {
            _env = env;
            _articleService = articleService;
            _categoryService = categoryService;
            _tagService = tagService;
            _cacheService = cacheService;
            _operationService = operationService;
        }
        public async Task<IActionResult> IndexAsync(ArticlePagedQuery query)
        {
            var items = await _articleService.GetAsync(query);
            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = categories;
            ViewBag.Query = query;
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> EditAsync(int? id)
        {
            var article = new ArticleRequest();
            if (id.HasValue)
            {
                var item = await _articleService.GetByIdAsync(id.Value);
                if (item != null)
                {
                    article.ArticleContent = item.ArticleContent;
                    article.Status = (EnumArticleStatuses)item.Status;
                    article.Title = item.Title;
                    article.Description = item.Description;
                    article.CategoryId = item.CategoryId;
                    article.Id = item.Id;
                    article.Thumbnail = item.Thumbnail;
                    article.IsTop = item.IsTop;
                    var tags = await _tagService.GetArticleTagsAsync(item.Id);
                    article.Tags = string.Join(",", tags.Select(x => x.Text).ToArray());
                }
            }
            ViewBag.Categories = await _categoryService.GetAllAsync();
            ViewBag.Tags = await _tagService.GetAllAsync();
            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync(ArticleRequest request)
        {
            if (!ModelState.IsValid)
                return this.BadRequest();
            var userState = Request.GetUserState();
            await GenerateThumbnailAsync(request);
            var success = false;
            if (request.Id.HasValue)
            {
                success = await _articleService.UpdateAsync(request);
                if (success)
                {
                    await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Update, EnumOperationReferenceTypes.Article, request.Id, request.Title);
                    await _tagService.UpdateArticleTagsAsync(request.Tags ?? string.Empty, request.Id.Value);
                }
            }
            else
            {
                success = await _articleService.CreateAsync(request, userState.Id);
                if (success && request.Id.HasValue)
                {
                    await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Create, EnumOperationReferenceTypes.Article, request.Id, request.Title);
                    await _tagService.CreateArticleTagsAsync(request.Tags ?? string.Empty, request.Id.Value);
                }
            }
            return success ? this.Success(request) : this.InternalServerError();
        }
        [HttpDelete]
        public async Task<IActionResult> IndexAsync(int id)
        {
            var curr = await _articleService.GetByIdAsync(id);
            if (curr == null)
            {
                return this.NotFound();
            }
            var result = await _articleService.DeleteByIdAsync(id);
            if (result)
            {
                var userState = Request.GetUserState();
                await _operationService.CreateAsync(userState.Id, EnumOperationTypes.Delete, EnumOperationReferenceTypes.Article, id, curr.Title);
                return this.Success();
            }
            return this.InternalServerError();
        }
        private async Task GenerateThumbnailAsync(ArticleRequest request)
        {
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var file = HttpContext.Request.Form.Files[0];
                var extension = Path.GetExtension(file.FileName);
                var newFileName = $"{DateTime.Now.ToString("HHmmssfff")}{extension}";
                var path = $"{DateTime.Now.ToString("yyyyMMdd")}";
                var url = $"/upload/{path}/{newFileName}";
                var filePath = $"{_env.WebRootPath}\\upload\\{path}";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                using (FileStream filestream = System.IO.File.Create($"{filePath}\\{newFileName}"))
                {
                    await file.CopyToAsync(filestream);
                    filestream.Flush();
                }
                request.Thumbnail = url;
            }
            if (!string.IsNullOrWhiteSpace(request.Thumbnail))
                return;
            int targetWidth = 240;
            int targetHeight = 160;
            int newWidth = 0;
            int newHeight = 0;
            Regex reg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>");
            MatchCollection ms = reg.Matches(request.ArticleContent ?? string.Empty);
            if (ms.Count <= 0)
                return;
            foreach (Match m in ms)
            {
                string firstImg = m.Groups["imgUrl"].Value;
                string imageUrl = firstImg;
                using (HttpClient wc = new HttpClient())
                {
                    if (!(imageUrl.ToLower().StartsWith("http://") || imageUrl.ToLower().StartsWith("https://")))
                    {
                        imageUrl = $"{Request.Scheme}://{Request.Host}{imageUrl}";
                    }
                    using (var response = await wc.GetAsync(imageUrl))
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new SKManagedStream(stream))
                    using (var bitmap = SKBitmap.Decode(fileStream))
                    {
                        bool needCreate = false;
                        if (bitmap.Width >= targetWidth)
                        {
                            newWidth = targetWidth;
                            newHeight = (targetWidth * bitmap.Height) / bitmap.Width;
                            if (newHeight >= targetHeight)
                            {
                                needCreate = true;
                            }
                        }
                        if (!needCreate)
                        {
                            if (bitmap.Height >= targetHeight)
                            {
                                newHeight = targetHeight;
                                newWidth = (targetHeight * bitmap.Width) / bitmap.Height;
                                if (newWidth >= targetWidth)
                                {
                                    needCreate = true;
                                }
                            }
                        }

                        if (needCreate)
                        {
                            //开始操作
                            using (var resized = bitmap.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Medium))
                            using (var newImage = SKImage.FromBitmap(resized))
                            using (var surface = SKSurface.Create(new SKImageInfo(targetWidth, targetHeight)))
                            {
                                var canvas = surface.Canvas;
                                if (newWidth == targetWidth)
                                {
                                    canvas.DrawImage(newImage, new SKPoint(0, (targetHeight - newHeight) / 2));
                                }
                                else
                                {
                                    canvas.DrawImage(newImage, new SKPoint((targetWidth - newWidth) / 2, 0));
                                }
                                string guid = Guid.NewGuid().ToString().Replace("-", string.Empty);
                                string extension = ".jpg";
                                string date = DateTime.Now.ToString("yyyyMMdd");
                                string path = $"{_env.WebRootPath}\\upload\\{date}\\";
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                using (var finalImage = surface.Snapshot())
                                {
                                    using (var writeStream = System.IO.File.OpenWrite(path + guid + extension))
                                    {
                                        finalImage.Encode(SKEncodedImageFormat.Jpeg, 80).SaveTo(writeStream);
                                    }
                                }
                                request.Thumbnail = "/upload/" + date + "/" + guid + extension;
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}