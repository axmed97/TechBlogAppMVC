using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechBlogAppMVC.Data;
using TechBlogAppMVC.Models;

namespace TechBlogAppMVC.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin, Admin Editor, Editor")]
    public class TagController : Controller
    {
        private readonly AppDbContext _context;

        public TagController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(_context.Tags.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Tag tag)
        {
            if (!ModelState.IsValid)
            {
                return View(tag);
            }

            var checktag = _context.Tags.FirstOrDefault(x => x.TagName == tag.TagName);

            if(checktag != null)
            {
                ViewBag.Error = "Tag name is exist";
                return View(tag);
            }

            _context.Tags.Add(tag);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
