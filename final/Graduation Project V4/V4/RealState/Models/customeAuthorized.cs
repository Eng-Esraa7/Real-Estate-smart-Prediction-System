using RealState.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RealState.Models
{
    public class customeAuthorized : AuthorizeAttribute
    {
        MySession session = MySession.Instance;
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Check if the user is authenticated
            if(session.user_id==null || session.user_id == "")
                return false;
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Redirect the user to the login page
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                { "action", "Login" },
                { "controller", "UserAccount" }
                });
        }
    }

}