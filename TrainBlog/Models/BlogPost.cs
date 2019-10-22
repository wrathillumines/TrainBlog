using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TrainBlog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Slug { get; set; }

        //[DataType(DataType.MultilineText)]
        public string Body { get; set; }
        public string MediaUrl { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy}", ApplyFormatInEditMode = true)]
        public DateTimeOffset Created { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMMM d, yyyy 'at' h:mm tt}", ApplyFormatInEditMode = true)]
        public DateTimeOffset? Updated { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public BlogPost()
        {
            Comments = new HashSet<Comment>();
        }
    }
}