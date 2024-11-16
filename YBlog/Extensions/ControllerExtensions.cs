using Microsoft.AspNetCore.Mvc;
using System.Net;
using YBlog.Models.Enums;

namespace YBlog.Extensions
{
    public static class ControllerExtensions
    {
        public static ConflictObjectResult Error(this Controller controller, EnumErrorCodes errorCode) {
            return controller.Conflict(new
            {
                ErrorCode = errorCode,
                Message = errorCode.GetDescription()
            });
        }
        public static JsonResult Success(this Controller controller, object? data = null)
        {
            return controller.Json(data);
        }
        private static JsonResult Error(Controller controller, HttpStatusCode statusCode, object? data = null)
        {
            controller.Response.StatusCode = (int)statusCode;
            return controller.Json(data);
        }
        public static JsonResult InternalServerError(this Controller controller, object? data = null)
        {
            return Error(controller, HttpStatusCode.InternalServerError, data);
        }
    }
}
