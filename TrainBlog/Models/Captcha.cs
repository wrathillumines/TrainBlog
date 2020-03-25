using Newtonsoft.Json;
using System.Collections.Generic;
namespace TrainBlog.Models
{
    public class Captcha
    {
        [JsonProperty("success")]
        public bool Success
        { get; set; }
        [JsonProperty("error-codes")]
        public List<string> ErrorMessage
        { get; set; }
    }
}