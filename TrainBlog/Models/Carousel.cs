namespace TrainBlog.Models
{
    public class Carousel
    {
        public int Id { get; set; }
        public string CreatorId { get; set; }
        public string ImageUrl { get; set; }
        public virtual ApplicationUser Creator { get; set; }
    }
}