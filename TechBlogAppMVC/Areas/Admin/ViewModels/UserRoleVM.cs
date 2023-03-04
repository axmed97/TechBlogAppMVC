using TechBlogAppMVC.Models;

namespace TechBlogAppMVC.Areas.Admin.ViewModels
{
    public class UserRoleVM
    {
        public User User { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
    