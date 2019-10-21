using System.ComponentModel.DataAnnotations;

namespace TrainBlog.Models
{
    public class EmailModel
    {
        [Required, Display(Name = "Name")]
        public string FromName { get; set; }
        [Required, Display(Name = "Email"), EmailAddress]
        public string FromEmail { get; set; }
        [Required, Display(Name = "Subject")]
        public string Subject { get; set; }
        [Required, Display(Name = "Message")]
        public string Body { get; set; }
    }
}