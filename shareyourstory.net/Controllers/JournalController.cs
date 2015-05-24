using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using shareyourstory.net.Controllers.Helpers;
using shareurstorydb;
using PagedList;
using DotNetOpenAuth.AspNet;

namespace shareyourstory.net.Controllers
{
    [Authorize]
    public class JournalController : BaseController
    {
        private bool _loggedIn = false;

        public JournalController()
            : base()
        {

            _loggedIn = true;
        }

        [Authorize]
        public ActionResult Index(int? page)
        {
            try
            {
                UserProfile user = (UserProfile)Session["User"];

                if (user == null || user.isActive == false)
                    return RedirectToAction("Manage", "Account");
                
                if (!page.HasValue)
                    page = 1;
                int pageNumber = (page ?? 1);
                var UserJournals = getUserJournals(User.UserId);
                ViewBag.CurrentUser = User;
                ControllerHelpers.DisconnectDB(DbContext);
                return View(UserJournals.ToPagedList(pageNumber, 5));
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
                return null;
            }
        }

        public ActionResult View(Int32 id)
        {
            try
            {
                return View(getUserJournal(id));
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                return null;
            }
        }
        public ActionResult Edit(Int32 id)
        {
            try
            {
                UserJournal journal = getUserJournal(id);
                return View(journal);
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
                return null;
            }
        }

        public ActionResult Create()
        {
            try
            {
                UserJournal post = new UserJournal()
                {
                    ID = 0,
                    Title = "",
                    Post = "",
                    UserId = User.UserId,
                    isActive = true
                };
                return View("Edit", post);
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                return View();
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(FormCollection post)
        {
            try
            {
                int id = Convert.ToInt32(post["Id"]);
                var qry = (from p in DbContext.UserJournals
                           where p.ID == id
                           select p).FirstOrDefault();
                UserJournal savePost = new UserJournal();
                savePost.ID = id;
                savePost.Post = SanitizeHtml.Sanitize(post["Post"]);
                savePost.CreateDate = DateTime.Now;
                savePost.Title = post["Title"];
                try
                {
                    savePost.UpdateDate = Convert.ToDateTime(post["UpdateDate"]);
                }
                catch (Exception)
                {
                    savePost.UpdateDate = DateTime.Now;
                }
                savePost.UserId = User.UserId;
                if (post["isActive"] != "false")
                    savePost.isActive = true;
                else
                    savePost.isActive = false;

                if (qry != null)
                {
                    qry.Post = savePost.Post;
                    qry.Title = savePost.Title;
                    qry.UpdateDate = savePost.UpdateDate;
                    qry.UserId = savePost.UserId;
                    qry.isActive = savePost.isActive;
                }
                else
                    DbContext.UserJournals.Add(savePost);

                int saveId = DbContext.SaveChanges();

                return RedirectToAction("Edit", new { id = savePost.ID });
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
                return View();
            }
        }

        private ActionResult gotoLoginPage(string retURL)
        {
            return RedirectToAction("index", "Login", new { returl = retURL });
        }


        private UserJournal getUserJournal(Int32 id)
        {
            try
            {
                return (from p in DbContext.UserJournals
                        where p.ID == id
                        select p).FirstOrDefault<UserJournal>();
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                return null;
            }

        }
        private IEnumerable<UserJournal> getUserJournals(Int32 userId)
        {
            try
            {
                return (from posts in DbContext.UserJournals
                        where posts.UserId == userId
                        orderby posts.UpdateDate descending
                        select posts);
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
                return null;
            }
        }
        private IEnumerable<UserJournal> getUserJournalsShort(Int32 userId)
        {

            try
            {
                return (from posts in DbContext.UserJournals
                        where posts.UserId == userId
                        orderby posts.UpdateDate descending
                        select posts);
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
                return null;
            }
        }
    }
}
