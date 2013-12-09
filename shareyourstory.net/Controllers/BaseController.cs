using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using shareyourstory.net.Controllers.Helpers;
using shareurstorydb;
using DotNetOpenAuth.AspNet;
using WebMatrix.WebData;
using Microsoft.Web.WebPages.OAuth;

namespace shareyourstory.net.Controllers
{
    public class BaseController : Controller
    {
        public MyStoryContext DbContext { get; private set; }
        public UserProfile User { get; private set; }
        public bool IsAuthorized { get; private set;}
        public BaseController()
        {
            ControllerHelpers.SetDbInitializer();
            DbContext = new MyStoryContext();
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (WebSecurity.IsAuthenticated)
            {
                if (Session != null && Session["user"] != null)
                {
                    User = (UserProfile)Session["user"];
                }
                else
                {
                    using (MyStoryContext db = new MyStoryContext())
                    {
                        User = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == WebSecurity.CurrentUserName.ToLower());
                    }
                    Session["user"] = User;
                }
            }
            //if (Session["User"] != null)
            //    User = (UserProfile)Session["User"];
            base.OnActionExecuting(filterContext);

            //this.OpenContext();
        }


        protected override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
            this.CloseContext();
        }

        public void OpenContext()
        {
            ControllerHelpers.OpenDb(DbContext);
        }

        public void CloseContext()
        {
            ControllerHelpers.DisconnectDB(DbContext);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DbContext = null;
        }

    }
}
