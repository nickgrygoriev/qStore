using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace qStore.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SimpleAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authCookie = httpContext.Request.Cookies["credentials"];

            //Check if user available to see content
            if (authCookie != null && (string.IsNullOrEmpty(Users) || Users.Contains(authCookie.Value)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.Cookies.IsAuthenticated())
            {
                filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
            }
            else
            {
                //Redirecting user to url that he wanted to see
                var dict = new Dictionary<string, object>();
                dict.Add("returnUrl", filterContext.RequestContext.HttpContext.Request.Url);

                filterContext.Result = new RedirectToRouteResult("Auth", new RouteValueDictionary(dict));
            }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }

    //External helper
    public static class AuthHelper
    {
        public static bool IsAuthenticated(this HttpCookieCollection cookies)
        {
            return cookies["credentials"] != null;
        }
    }
}