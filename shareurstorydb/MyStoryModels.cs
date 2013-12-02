using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace shareurstorydb
{
    public class UserPost
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public String Title { get; set; }
        public String Post { get; set; }
        public String Name { get; set; }
        public int Likes { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }

    //public class Users
    //{
    //    public int ID { get; set; }
    //    public string FB_UserID { get; set; }
    //    public string Firstname { get; set; }
    //    public string Lastname { get; set; }
    //    public string Password { get; set; }
    //    public string Email { get; set; }
    //    public string DOB { get; set; }
    //    public string Gender { get; set; }
    //    public string Country { get; set; }
    //    public string FB_SignedRequest { get; set; }
    //    public string FB_SignedRequestDecoded { get; set; }
    //    public DateTime CreateDate { get; set; }
    //    public DateTime UpdateDate { get; set; }
    //}

    public class Tags
    {
        public int ID { get; set; }
        public string TagText { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class PostTags
    {
        public int ID { get; set; }
        public int UserPostID { get; set; }
        public int UserID { get; set; }
        public int TagID { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class PostLikes
    {
        public int ID { get; set; }
        public int PostID { get; set; }
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class TopPostLikes
    {
        public TopPostLikes() { }
        public UserPost post { get; set; }
        public int numberLikes { get; set; }
    }

    public class PostShares
    {
        public int ID { get; set; }
        public int PostID { get; set; }
        public int UserID { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class PostComments
    {
        public int ID { get; set; }
        public int PostID { get; set; }
        public int UserID { get; set; }
        public UserProfile user { get; set; }
        public string Comment { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class PostReadings
    {
        public int ID { get; set; }
        public int PostID { get; set; }
        public int UserID { get; set; }
        public string IP { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class TopPostReadings
    {
        public TopPostReadings() { }
        public UserPost post { get; set; }
        public int numberReadings { get; set; }
    }

    public class CommentsDTO
    {
        public int ID { get; set; }
        public int PostID { get; set; }
        public int UserID { get; set; }
        public String Name { get; set; }
        public string Comment { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class StoriesDTO
    {
        public int ID { get; set; }
        public int UserId { get; set; }
        public String Title { get; set; }
        public String Post { get; set; }
        public String Name { get; set; }
        public int Likes { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class StoriesListModel
    {
        public List<StoriesDTO> Stories { get; set; }
        public List<string> PageNoList { get; set; }
        public string currentPage { get; set; }
        public IEnumerable<SelectListItem> SearchOptions { get; set; }
        public string SearchText { get; set; }
        public IEnumerable<SelectListItem> SearchCategories { get; set; }
        public IEnumerable<SelectListItem> SortOptions { get; set; }
        public string SortOption { get; set; }
        public List<CommentsDTO> Comments { get; set; }
    }

    public class Contacts
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class Logs
    {
        public int ID { get; set; }
        public string LogType { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
    }
}