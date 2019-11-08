namespace TrainBlog.Models
{
    public class AboutSnippet
    {
        public int Id { get; set; }
        public string CreatorId { get; set; }
        public string ImageUrl { get; set; }
        public string Text { get; set; }
        public virtual ApplicationUser Creator { get; set; }
    }
}