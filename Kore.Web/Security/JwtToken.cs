using System.Web.Mvc;

namespace Kore.Web.Security
{
    public class JwtToken : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var token = filterContext.HttpContext.Request.Cookies.Get("token");

            if (token != null)
            {
                filterContext.HttpContext.Request.Headers.Add("Authorization", "Bearer " + token.Value);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
