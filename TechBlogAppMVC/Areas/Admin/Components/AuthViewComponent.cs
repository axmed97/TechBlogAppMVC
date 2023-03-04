using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TechBlogAppMVC.Data;

namespace TechBlogAppMVC.Areas.Admin.Components
{
    public class AuthViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _context;

        public AuthViewComponent(IHttpContextAccessor contextAccessor, AppDbContext context)
        {
            _contextAccessor = contextAccessor;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            return View(user);
        }

    }
}
