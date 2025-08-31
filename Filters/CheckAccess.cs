using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace HospitalManagement.Filters
{
    public class CheckAccess : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var controller = filterContext.RouteData.Values["controller"]?.ToString();
            var action = filterContext.RouteData.Values["action"]?.ToString();

            // Allow unauthenticated access to specific User actions (login/register/logout)
            if (string.Equals(controller, "User", StringComparison.OrdinalIgnoreCase) &&
                (string.Equals(action, "Login", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(action, "Register", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(action, "Logout", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            if (filterContext.HttpContext.Session.GetString("UserID") == null)
            {
                filterContext.Result = new RedirectToActionResult("Login", "User", null);
            }
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            context.HttpContext.Response.Headers["Expires"] = "-1";
            context.HttpContext.Response.Headers["Pragma"] = "no-cache";
            base.OnResultExecuting(context);
        }
    }
}
