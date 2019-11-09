using System.Web.Mvc;

namespace TrainBlog.Models
{
    public class About
    {
        public int Id { get; set; }
        public string CreatorId { get; set; }
        public string ImageUrl { get; set; }

        [AllowHtml]
        public string Text { get; set; }
        public virtual ApplicationUser Creator { get; set; }
    }
}