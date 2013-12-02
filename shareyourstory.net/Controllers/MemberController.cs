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
    public class MemberController : BaseController
    {
        private bool _loggedIn = false;

        public MemberController()
            : base()
        {

            _loggedIn = true;
        }

        [Authorize]
        public ActionResult Index(int? page)
        {
            try
            {
                
                if (!page.HasValue)
                    page = 1;
                int pageNumber = (page ?? 1);
                var userPosts = getUserPosts(User.UserId);
                ViewBag.CurrentUser = User;
                ControllerHelpers.DisconnectDB(DbContext);
                return View(userPosts.ToPagedList(pageNumber, 5));
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
                return View(getUserPost(id));
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
                return View(getUserPost(id));
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
                UserPost post = new UserPost()
                {
                    ID = 0,
                    Title = "",
                    Post = "",
                    UserId = User.UserId
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
                var qry = (from p in DbContext.UserPosts
                           where p.ID == id
                           select p).FirstOrDefault();
                UserPost savePost = new UserPost();
                savePost.ID = id;
                savePost.Post = SanitizeHtml.Sanitize(post["Post"]);
                savePost.CreateDate = DateTime.Now;
                savePost.Title = post["Title"];
                savePost.UpdateDate = DateTime.Now;
                savePost.UserId = User.UserId;

                if (qry != null)
                {
                    qry.Post = savePost.Post;
                    qry.Title = savePost.Title;
                    qry.UpdateDate = DateTime.Now;
                    qry.UserId = savePost.UserId;
                }
                else
                    DbContext.UserPosts.Add(savePost);

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


        private UserPost getUserPost(Int32 id)
        {
            try
            {
                return (from p in DbContext.UserPosts
                        where p.ID == id
                        select p).FirstOrDefault<UserPost>();
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                return null;
            }

        }
        private IEnumerable<UserPost> getUserPosts(Int32 userId)
        {
            try
            {
                return (from posts in DbContext.UserPosts
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
