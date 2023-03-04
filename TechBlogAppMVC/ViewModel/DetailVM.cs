using TechBlogAppMVC.Models;

namespace TechBlogAppMVC.ViewModel
{
    public class DetailVM
    {
        public Article Article { get; set; }
        public List<Article> Suggestion { get; set; }
        public Article Before { get; set; }
        public Article After { get; set; }

    }
}
