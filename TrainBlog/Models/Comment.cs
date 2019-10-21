using System;
using System.Collections.Generic;

namespace TrainBlog.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public string AuthorId { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public virtual BlogPost BlogPost { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public Comment()
        {
            Notifications = new HashSet<Notification>();
        }
    }
}