using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using shareurstorydb;
using shareyourstory.net.Controllers.Helpers;
using System.Configuration;
using Facebook;
using System.Web.Services;

namespace shareyourstory.net.Controllers
{
    public class HomeController : BaseController
    {
        //Initialize Paging
        int PageCount = 10;
        int PageNumber = 1;
        string failMessage = "";
        //
        // GET: /Index/

        public HomeController()
            : base()
        {
            //Set the page size
            if (System.Configuration.ConfigurationManager.AppSettings["DefaultPageSize"] != null)
                if (int.TryParse(System.Configuration.ConfigurationManager.AppSettings["DefaultPageSize"], out PageCount) == false)
                    PageNumber = 10;
        }
        public ActionResult Index()
        {

            try
            {
                var latestPosts = Helpers.ControllerHelpers.GetLatestTopXUserPosts(3, DbContext);
                ViewBag.PopularPosts = ControllerHelpers.GetPopularTopXUserPosts(3, DbContext);
                ViewBag.TopRatedPosts = ControllerHelpers.GetTopRatedTopXUserPosts(3, DbContext);
                return View(latestPosts);
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
            }
            return View();
        }

        public ActionResult ViewPost(int id)
        {

            try
            {
                UserProfile user = (UserProfile)Session["User"];
                int userId = 0;
                if (user != null)
                    userId = user.UserId;
                ViewData["AppID"] = ConfigurationManager.AppSettings["AppID"];
                StoriesListModel stories = new StoriesListModel();
                List<StoriesDTO> storiesDTO = Helpers.ControllerHelpers.GetStory(DbContext, id, userId);
                stories.Stories = storiesDTO;
                List<CommentsDTO> comments = ControllerHelpers.GetStoryComments(DbContext, id);
                stories.Comments = comments;

                //Save reading
                MyStoryContext _context = new MyStoryContext();
                PostReadings read = new PostReadings();
                read.PostID = id;
                read.UserID = userId;
                read.IP = HttpContext.Request.UserHostAddress;
                read.CreateDate = DateTime.Now;
                _context.PostReadings.Add(read);
                _context.SaveChanges();

                return View(stories);
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
            }
            return null;

        }
        //
        // GET: /Index/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Index/Create

        public ActionResult Create(UserPost post)
        {
            DbContext.UserPosts.Add(post);
            DbContext.SaveChanges();
            return View();
        }

        //
        // POST: /Index/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
            }
            return View();
        }

        //
        // GET: /Index/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Index/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Index/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Index/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
                return View();
            }
        }

        [WebMethod]
        public string FBLike(string id)
        {
            try
            {
                int intid = Convert.ToInt32(id);
                UserProfile user = (UserProfile)Session["User"];

                MyStoryContext _context = new MyStoryContext();
                if ((from l in _context.PostLikes where l.PostID == intid && l.UserID == user.UserId select l).Count() == 0)
                {
                    PostLikes like = new PostLikes();
                    like.PostID = intid;
                    like.UserID = user.UserId;
                    like.CreateDate = DateTime.Now;

                    _context.PostLikes.Add(like);
                    _context.SaveChanges();
                }

                return "Success";
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
            }
            return "Fail";

        }

        [WebMethod]
        public string FBUnlike(string id)
        {
            try
            {
                int intid = Convert.ToInt32(id);
                UserProfile user = (UserProfile)Session["User"];

                MyStoryContext _context = new MyStoryContext();
                if ((from l in _context.PostLikes where l.PostID == intid && l.UserID == user.UserId select l).Count() > 0)
                {
                    PostLikes like = _context.PostLikes.Where(l => l.PostID == intid && l.UserID == user.UserId).Single();
                    _context.PostLikes.Remove(like);
                    _context.SaveChanges();
                }

                return "Success";
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
            }
            return "Fail";
        }

        [WebMethod]
        public string FBShare(string id)
        {
            try
            {
                int intid = Convert.ToInt32(id);
                UserProfile user = (UserProfile)Session["User"];

                MyStoryContext _context = new MyStoryContext();
                PostShares share = new PostShares();
                share.PostID = intid;
                share.UserID = user.UserId;
                share.CreateDate = DateTime.Now;

                _context.PostShares.Add(share);
                _context.SaveChanges();
                return "Success";
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
            }
            return null;
        }

        #region Stories
        public ActionResult stories()//FormCollection collection)
        {
            string sortOption = Request.QueryString["SortOption"];
            string searchText = Request.QueryString["SearchText"];
            int userId = 0;
            if (int.TryParse(Request.QueryString["u"], out userId) == false)
                userId = 0;
            StoriesListModel stories = new StoriesListModel();
            try
            {
                // Initialize the cache and retrieve stories.
                stories = doMemoryAndGetStories(sortOption, searchText, userId);
                stories.PageNoList = new List<string>();
                //Initialize the stories properties
                PopulateDDL(stories);
                //Do Searching
                Searching(stories, searchText);//collection);
                //Do Sorting
                Sorting(stories, sortOption);//collection);
                //Do Paging
                Paging(stories);

                return View(stories);
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
                ControllerContext.RequestContext.HttpContext.Trace.Write(ex.ToString());
                return View(stories);
            }
        }
        [HttpGet]
        public string Follow()
        {
            string retValue = "success";
            try
            {
                int followedUserId = 0;
                UserProfile user = (UserProfile)Session["User"];
                if (int.TryParse(Request.RawUrl.Split('/')[3], out followedUserId) == false)
                    throw new Exception("Followed user identifier could not be found.");
                DbContext.UserFollows.Add(new UserFollow()
                {
                    FollowedUserId = followedUserId,
                    UserId = user.UserId,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                });

                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                retValue = "fail";
            }
            return retValue;
        }
        [HttpGet]
        public string Favorite()
        {

            string retValue = "success";
            try
            {
                int storyId = 0;
                UserProfile user = (UserProfile)Session["User"];
                if (int.TryParse(Request.RawUrl.Split('/')[3], out storyId) == false)
                    throw new Exception("Favorite story identifier could not be found.");
                DbContext.UserFavorites.Add(new UserFavorite()
                {
                    StoryId = storyId,
                    UserId = user.UserId,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                });

                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                retValue = "fail";
            }
            return retValue;
        }
        [HttpGet]
        public string FollowDel()
        {
            string retValue = "success";
            try
            {
                int followedUserId = 0;
                UserProfile user = (UserProfile)Session["User"];
                if (int.TryParse(Request.RawUrl.Split('/')[3], out followedUserId) == false)
                    throw new Exception("Followed user identifier could not be found.");
                var follow = (from f in DbContext.UserFollows
                             where f.FollowedUserId == followedUserId
                                && f.UserId == user.UserId
                             select f).FirstOrDefault<UserFollow>();
                if (follow == null)
                    throw new Exception("Follow instance could not be found.");
                DbContext.UserFollows.Remove(follow);

                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                retValue = "fail";
            }
            return retValue;

        }
        [HttpGet]
        public string FavoriteDel()
        {
            string retValue = "success";
            try
            {
                int storyId = 0;
                UserProfile user = (UserProfile)Session["User"];
                if (int.TryParse(Request.RawUrl.Split('/')[3], out storyId) == false)
                    throw new Exception("Favorite story identifier could not be found.");
                var fave = (from f in DbContext.UserFavorites
                             where f.StoryId == storyId
                                && f.UserId == user.UserId
                             select f).FirstOrDefault<UserFavorite>();
                if (fave == null)
                    throw new Exception("Favorite instance could not be found.");
                DbContext.UserFavorites.Remove(fave);

                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                retValue = "fail";
            }
            return retValue;
        }
        private StoriesListModel doMemoryAndGetStories(string sortOption, string searchText, int userID = 0)
        {
            StoriesListModel stories = new StoriesListModel();
            StoriesListModel retStories = new StoriesListModel();
            stories.SortOption = sortOption;
            stories.SearchText = searchText;
            UserProfile user = (UserProfile)Session["User"];


            //Get All Stories
            List<StoriesDTO> storiesDTO = Helpers.ControllerHelpers.GetStories(DbContext, user.UserId);
            stories.Stories = storiesDTO;


            retStories = stories;
            if (userID > 0)
                retStories.Stories = (from s in stories.Stories
                                      where s.UserId == userID
                                      select s).ToList<shareurstorydb.StoriesDTO>();

            return retStories;
        }

        private StoriesListModel PopulateDDL(StoriesListModel stories)
        {
            stories.SearchOptions = new[] {
                    new SelectListItem { Value = "0", Text = "Search All" },
                    new SelectListItem { Value = "1", Text = "Category" },
                    new SelectListItem { Value = "2", Text = "Story Name" }
                };

            stories.SortOptions = new[] {
                    new SelectListItem { Value = "0", Text = "Most Recent" },
                    new SelectListItem { Value = "1", Text = "Alphabetical" },
                    new SelectListItem { Value = "2", Text = "Most Likes" }
                };

            stories.SearchCategories = new[] {
                    new SelectListItem { Value = "0", Text = "All Categories" },
                    new SelectListItem { Value = "1", Text = "test 1" },
                    new SelectListItem { Value = "2", Text = "test 2" }
                };

            return stories;
        }


        private StoriesListModel Searching(StoriesListModel stories, string searchText)//FormCollection collection)
        {
            try
            {
                if (searchText != null && searchText != "")
                    stories.Stories = stories.Stories.Where(s1 => (s1.Name.ToLower().Contains(searchText.ToLower()) || s1.Title.ToLower().Contains(searchText.ToLower()))).ToList();
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
            }

            return stories;
        }

        private StoriesListModel Sorting(StoriesListModel stories, string sortOption)//FormCollection collection)
        {

            try
            {
                //string SortOption = "";
                //SortOption = collection.GetValue("SortOptions").AttemptedValue;

                if (sortOption == "0")
                    stories.Stories = stories.Stories.OrderByDescending(s => s.CreateDate).ToList();
                else if (sortOption == "1")
                    stories.Stories = stories.Stories.OrderBy(s => s.Name).ToList();
                else if (sortOption == "2")
                    stories.Stories = stories.Stories.OrderByDescending(s => s.Likes).ToList();
                else
                    stories.Stories = stories.Stories.OrderByDescending(s => s.CreateDate).ToList();
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
            }

            return stories;
        }

        private StoriesListModel Paging(StoriesListModel stories)
        {
            try
            {
                if (Request.QueryString["PageNumber"] != "" && Request.QueryString["PageNumber"] != null)
                    PageNumber = Convert.ToInt32(Request.QueryString["PageNumber"]);


                if (PageNumber != 0)
                {
                    int pages = Convert.ToInt32(Math.Ceiling((double)stories.Stories.Count / (double)PageCount));
                    for (int i = 1; i <= pages; i++) //Get number of pages
                    {
                        stories.PageNoList.Add(i.ToString());
                    }
                    if (stories.Stories.Skip(PageCount * (PageNumber - 1)).Take(PageCount).ToList().Count > 0)
                    {
                        stories.Stories = stories.Stories.Skip(PageCount * (PageNumber - 1)).Take(PageCount).ToList();
                        stories.currentPage = PageNumber.ToString();
                    }
                    else
                    {
                        stories.Stories = stories.Stories.Skip(0).Take(PageCount).ToList();
                        stories.currentPage = "1";
                    }
                }
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
            }
            return stories;
        }


        //Should we request them to login, if so then should I maybe implement a popup with Facebook login (Popup saying: Please login using Facebook: then have the Facebook login like in the right menu)?
        //Why does it wrap the comment box between curly brakets and say Path: at bottom?
        //Do you have the code to ignore HTML tags?
        //Also the FBShare and FBLike, we save the userid, should we also require login? Maybe we should require login when they click on see more on a story?
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ViewPost(StoriesListModel story, FormCollection post)
        {
            try
            {
                UserProfile user = (UserProfile)Session["User"];

                if (user.isActive == false)
                    return RedirectToAction("Manage", "Account");

                //string comment = "test test test 123..."; // Request.QueryString["txtComment"];
                string comment = SanitizeHtml.Sanitize(post["txtComment"]);
                Int32 PostID = story.Stories[0].ID;

                PostComments p = new PostComments();
                p.UserID = user.UserId;
                p.PostID = PostID;
                p.Comment = comment;
                p.CreateDate = DateTime.Now;

                DbContext.PostComments.Add(p);
                DbContext.SaveChanges();

                ViewData["AppID"] = ConfigurationManager.AppSettings["AppID"];
                List<StoriesDTO> storiesDTO = Helpers.ControllerHelpers.GetStory(DbContext, PostID, user.UserId);
                story.Stories = storiesDTO;
                List<CommentsDTO> comments = ControllerHelpers.GetStoryComments(DbContext, PostID);
                story.Comments = comments;
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                //ViewData["ErrorMsg"] = failMessage;
                return RedirectToAction("Login", "Account");
            }
            return View(story);
        }
        #endregion

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Contact(FormCollection post)
        {
            string displaymessage = "Message was sent successfully.";

            try
            {
                UserProfile user = new UserProfile();
                user.UserId = 0;
                try
                {
                    if (Session["User"] != null)
                        user = (UserProfile)Session["User"];
                }
                catch (Exception)
                {
                }

                //string comment = "test test test 123..."; // Request.QueryString["txtComment"];
                string subject = SanitizeHtml.Sanitize(post["txtSubject"]);
                string message = SanitizeHtml.Sanitize(post["txtMessage"]);

                Contacts c = new Contacts();
                c.UserID = user.UserId;
                c.Subject = subject;
                c.Message = message;
                c.CreateDate = DateTime.Now;

                DbContext.Contacts.Add(c);
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                ControllerHelpers.LogError(DbContext, ex, out failMessage);
                ViewData["ErrorMsg"] = failMessage;
                displaymessage = "An error occured, please try again.";
            }

            Response.Write("<script langauge='javascript'>alert('" + displaymessage + "');</script>");
            return View();
        }
    }
}
