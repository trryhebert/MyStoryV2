using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace shareyourstory.net.Controllers
{
    public class ErrorController : BaseController
    {


        public ActionResult HttpError()
        {
            Exception ex = null;
            string failureMsg = "";
            try
            {
                ex = (Exception)HttpContext.Application[Request.UserHostAddress.ToString()];
            }
            catch
            {
            }

            try
            {
                if (Helpers.ControllerHelpers.LogError(DbContext, ex, out failureMsg) != true)
                    ViewData["Description"] = failureMsg;
            }
            catch
            { }
            if (ex != null)
            {
                ViewData["Description"] = ex.Message;
            }
            else
            {
                ViewData["Description"] = "An error occurred.";
            }

            ViewData["Title"] = "Oops. We're sorry. An error occurred and we're on the case.";

            return View("Error");
        }

        public ActionResult NotFound()
        {
            Exception ex = null;
            string failureMsg = "";
            try
            {
                ex = (Exception)HttpContext.Application[Request.UserHostAddress.ToString()];
            }
            catch
            {
            }
            ViewData["Title"] = "The page you requested was not found";
            try
            {
                if (Helpers.ControllerHelpers.LogError(DbContext, ex, out failureMsg) != true)
                    ViewData["Description"] = failureMsg;
            }
            catch
            { }

            return View("Error");
        }

        // (optional) Redirect to home when /Error is navigated to directly
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

    }
}
