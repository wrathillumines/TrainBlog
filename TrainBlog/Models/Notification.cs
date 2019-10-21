using System;

namespace TrainBlog.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public string RecipientId { get; set; }
        public string SenderId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public bool HasBeenRead { get; set; }
        public virtual ApplicationUser Recipient { get; set; }
        public virtual ApplicationUser Sender { get; set; }
    }
}