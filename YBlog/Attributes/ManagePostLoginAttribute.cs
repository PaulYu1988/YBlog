using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using YBlog.Extensions;

namespace YBlog.Attributes
{
    public class ManagePostLoginAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {

        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var userState = context.HttpContext.Request.GetUserState();
            if (!userState.IsLogin
                || (userState.Type == Models.Enums.EnumUserTypes.Common))
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
                    context.Result = new RedirectResult("/login");
                }
            }
        }
    }
}
