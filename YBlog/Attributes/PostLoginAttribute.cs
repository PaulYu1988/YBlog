using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using YBlog.Extensions;
using Microsoft.AspNetCore.Http.Extensions;

namespace YBlog.Attributes
{
    public class PostLoginAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var userState = context.HttpContext.Request.GetUserState();
            if (!userState.IsLogin)
            {
                if (context.HttpContext.Request.IsAjax())
                {
                    context.Result = new JsonResult(null)
                    {
                        StatusCode = (int)HttpStatusCode.Unauthorized
                    };
                }
                else
                {
                    context.Result = new RedirectResult($"/login?from={context.HttpContext.Request.GetEncodedUrl()}");
                }
            }
        }
    }
}
