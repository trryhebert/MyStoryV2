using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using shareyourstory.net.Controllers.Helpers;
using shareurstorydb;
using shareyourstory.net.Models;

namespace shareyourstory.net.Controllers
{
    public class BaseController : Controller
    {
        public MyStoryContext DbContext { get; private set; }
        public UserProfile User { get; private set; }
        public BaseController()
        {
            ControllerHelpers.SetDbInitializer();
            DbContext = new MyStoryContext();
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (Session["User"] != null)
                User = (UserProfile)Session["User"];
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
