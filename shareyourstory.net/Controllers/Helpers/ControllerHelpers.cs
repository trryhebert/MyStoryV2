using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using shareurstorydb;
using System.Data.Entity;

namespace shareyourstory.net.Controllers.Helpers
{
    public class ControllerHelpers
    {
        //public static Models.Repository GetRepo()
        //{
        //    return new Models.Repository();
        //    //return new Models.Repository(ConfigurationManager.ConnectionStrings["Prod"].ToString());
        //}
        public static void SetDbInitializer()
        {
            Database.SetInitializer<MyStoryContext>(new CreateDb());
        }
        public static void OpenDb(MyStoryContext context)
        {
            if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                context.Database.Connection.Open();
        }
        public static void DisconnectDB(MyStoryContext context)
        {
            if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                context.Database.Connection.Close();
        }
        public static IQueryable<UserPost> GetLatestTopXUserPosts(int topNum, MyStoryContext context)
        {
            return (from p in context.UserPosts
                    join o in context.UserProfiles on p.UserId equals o.UserId
                    where p.isActive == true && o.isActive == true
                    select p).Take<UserPost>(topNum);
        }
        public static IQueryable<TopPostReadings> GetPopularTopXUserPosts(int topNum, MyStoryContext context)
        {
            var topreadings = (from p in context.UserPosts
                               from r in context.PostReadings
                               join o in context.UserProfiles on p.UserId equals o.UserId
                               where r.PostID == p.ID && p.isActive == true && o.isActive == true
                               group r.IP by p into pr
                               select new TopPostReadings { post = pr.Key, numberReadings = pr.Distinct().Count() }
                   ).OrderByDescending(x => x.numberReadings).Take(topNum);

            return topreadings;
        }
        public static IQueryable<TopPostLikes> GetTopRatedTopXUserPosts(int topNum, MyStoryContext context)
        {
            //http://stackoverflow.com/questions/1387110/linq-to-sql-get-top-10-most-ordered-products
            var topposts = (from p in context.UserPosts
                            from l in context.PostLikes
                            join o in context.UserProfiles on p.UserId equals o.UserId
                            where l.PostID == p.ID && p.isActive == true && o.isActive == true
                            group l by p into pl
                            select new TopPostLikes { post = pl.Key, numberLikes = pl.Count() }
                   ).OrderByDescending(x => x.numberLikes).Take(topNum);

            return topposts;
        }

        public static List<StoriesDTO> GetStories(MyStoryContext context, int userId = 0)
        {
            return (from p in context.UserPosts
                    //where usrs.LastActivityDate <= duration 
                    join o in context.UserProfiles on p.UserId equals o.UserId
                    where p.isActive == true && o.isActive == true
                    select new StoriesDTO()
                    {
                        ID = p.ID,
                        UserId = p.UserId,
                        Title = p.Title,
                        Post = p.Post,
                        Name = o.UserName, //o.Firstname + " " + o.Lastname,
                        CreateDate = p.CreateDate,
                        Likes = (from l in context.PostLikes where l.PostID == p.ID select l).Count(),
                        FaveInd = ((from f in context.UserFavorites where f.StoryId == p.ID && f.UserId == userId select f).Count() > 0),
                        FollowInd = ((from f in context.UserFollows where f.FollowedUserId == p.UserId && f.UserId == userId select f).Count() > 0),
                    }
                    ).ToList();
        }

        public static List<StoriesDTO> GetStory(MyStoryContext context, int storyId, int currentUserId)
        {
            bool faveInd = false;
            if (currentUserId > 0 && (from f in context.UserFavorites where f.StoryId == storyId && f.UserId == currentUserId select f).Count() > 0)
                faveInd = true;
            return (from p in context.UserPosts
                    //where usrs.LastActivityDate <= duration 
                    join o in context.UserProfiles on p.UserId equals o.UserId
                    where p.ID == storyId && p.isActive == true && o.isActive == true
                    select new StoriesDTO()
                    {
                        ID = p.ID,
                        UserId = p.UserId,
                        Title = p.Title,
                        Post = p.Post,
                        Name = o.UserName, //o.Firstname + " " + o.Lastname,
                        CreateDate = p.CreateDate,
                        Likes = (from l in context.PostLikes where l.PostID == p.ID select l).Count(),
                        FaveInd = faveInd,
                        FollowInd = ((from f in context.UserFollows where f.FollowedUserId == p.UserId && f.UserId == currentUserId select f).Count() > 0)
                    }
                    ).ToList();
        }

        public static List<CommentsDTO> GetStoryComments(MyStoryContext context, int id)
        {
            //List<PostComments> comments = (from l in context.PostComments where l.PostID == id select l).ToList();
            ////user????
            //return comments;
            ////return (from l in context.PostComments join o in context.Users on l.UserID equals o.ID where l.PostID == id select l).ToList();



            return (from p in context.PostComments
                    where p.PostID == id
                    join o in context.UserProfiles on p.UserID equals o.UserId
                    where o.isActive == true
                    select new CommentsDTO()
                    {
                        ID = p.ID,
                        PostID = p.ID,
                        UserID = p.UserID,
                        Name = o.UserName, //o.Firstname + " " + o.Lastname,
                        Comment = p.Comment,
                        CreateDate = p.CreateDate
                    }
                    ).ToList();
        }
        /// <summary>
        /// Logs whatever error to the database. Returns true or false for success or failure.  
        /// Also returns a fail message if the transaction failed
        /// </summary>
        /// <param name="context">The database context</param>
        /// <param name="ex">The exception that was thrown</param>
        /// <param name="failMessage">The message if the transaction failed</param>
        /// <returns></returns>
        public static bool LogError(MyStoryContext context, Exception ex, out string failMessage)
        {
            bool retVal = true;
            failMessage = "";
            try
            {
                Logs errorLog = new Logs();
                String msg = "";
                Exception tmpEx = ex;

                while (tmpEx != null)
                {
                    msg += tmpEx.Message;
                    tmpEx = tmpEx.InnerException;
                }

                errorLog.CreateDate = DateTime.Now;
                errorLog.LogType = "error";
                errorLog.Message = msg;
                context.Logs.Add(errorLog);
                context.SaveChanges();
                throw ex;
            }
            catch (Exception innerEx)
            {
                retVal = false;
                failMessage = innerEx.Message;
            }
            return retVal;
        }
        public static bool LogError(MyStoryContext context, string message, out string failMessage)
        {
            bool retVal = true;
            failMessage = "";
            try
            {
                Logs errorLog = new Logs();

                errorLog.CreateDate = DateTime.Now;
                errorLog.LogType = "error";
                errorLog.Message = message;
                context.Logs.Add(errorLog);
                context.SaveChanges();

            }
            catch (Exception innerEx)
            {
                retVal = false;
                failMessage = innerEx.Message;
            }
            return retVal;
        }
        public static bool LogMessage(MyStoryContext context, string message, out string failMessage)
        {
            bool retVal = true;
            failMessage = "";
            try
            {
                Logs errorLog = new Logs();

                errorLog.CreateDate = DateTime.Now;
                errorLog.LogType = "message";
                errorLog.Message = message;
                context.Logs.Add(errorLog);
                context.SaveChanges();

            }
            catch (Exception innerEx)
            {
                retVal = false;
                failMessage = innerEx.Message;
            }
            return retVal;
        }
        public static IEnumerable<UserStoryListModel> GetUserFavorites(MyStoryContext context, int id)
        {
            try
            {
                return (from f in context.UserFavorites
                        join p in context.UserPosts on f.StoryId equals p.ID
                        join u in context.UserProfiles on f.UserId equals u.UserId
                        where f.UserId == id
                        orderby f.CreateDate descending
                        select new UserStoryListModel
                        {
                            StoryUserId = p.UserId,
                            StoryId = f.StoryId,
                            UserName = u.UserName,
                            StoryTitle = p.Title,
                            StorySample = p.Post.Substring(0, 50)
                        }).ToList<UserStoryListModel>();
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(context, ex, out failMessage);
                throw ex;
            }
        }
        public static List<UserStoryListModel> GetUserFollowed(MyStoryContext context, int id)
        {
            try
            {
                List<UserStoryListModel> lst = (from f in context.UserFollows
                                                join p in context.UserPosts on f.FollowedUserId equals p.UserId
                                                join u in context.UserProfiles on f.FollowedUserId equals u.UserId
                                                where f.UserId == id
                                                orderby f.CreateDate descending
                                                select new UserStoryListModel
                                                {
                                                    StoryUserId = f.FollowedUserId,
                                                    StoryId = p.ID,
                                                    UserName = u.UserName,
                                                    StoryTitle = p.Title,
                                                    StorySample = p.Post.Substring(0, 50)
                                                }).ToList<UserStoryListModel>();
                return lst;
            }
            catch (Exception ex)
            {
                string failMessage = "";
                ControllerHelpers.LogError(context, ex, out failMessage);
                throw ex;
            }
        }
    }
}