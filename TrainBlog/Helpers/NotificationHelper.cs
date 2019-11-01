using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using TrainBlog.Models;
using Microsoft.AspNet.Identity;

namespace TrainBlog.Helpers
{
    public class NotificationHelper : CommonHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void ManageNotifications(Comment comment)
        {
            CreateCommentNotification(comment);
        }

        public static void CreateCommentNotification(Comment comment)
        {
            GenerateCommentNotification(comment);
        }

        private static void GenerateCommentNotification(Comment comment)
        {
            var senderId = HttpContext.Current.User.Identity.GetUserId();
            var recipientId = Db.Users.FirstOrDefault(u => u.Email == "hwphotog@gmail.com").Id;
            var postId = comment.BlogPostId;
            var postTitle = Db.BlogPosts.Find(postId).Title.ToString();
            var notification = new Notification
            {
                Created = DateTime.Now,
                Subject = $"<hr />New Comment on \"{postTitle}\"",
                HasBeenRead = false,
                RecipientId = recipientId,
                SenderId = senderId,
                Body = $"A new comment was posted to \"{postTitle}\" on {DateTime.Now.ToShortDateString()} at {DateTime.Now.ToShortTimeString()}.",
                CommentId = comment.Id
            };
            Db.Notifications.Add(notification);
            Db.SaveChanges();
        }

        public static int GetNewUserNotificationCount()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return Db.Notifications.Where(t => t.RecipientId == userId && !t.HasBeenRead).Count();
        }

        public int GetAllUserNotificationCount()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return Db.Notifications.Where(t => t.RecipientId == userId).Count();
        }

        public static List<Notification> GetUnreadUserNotifications()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return Db.Notifications.Where(t => t.RecipientId == userId && !t.HasBeenRead).ToList();
        }

        public static List<Notification> ListUserNotifications()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            return Db.Notifications.ToList();
        }
    }
}