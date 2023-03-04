using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TechBlogAppMVC.Data;
using TechBlogAppMVC.Helpers;
using TechBlogAppMVC.Models;

namespace TechBlogAppMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int pg = 1)
        {
            const int pageSize = 9;
            if(pg < 1)
            {
                pg = 1;
            }
            
            int articleCount = _context.Articles.Count();
            
            var pager = new Pager(articleCount, pg, pageSize);

            int arcSkip = (pg - 1) * pageSize;

            var articles = _context.Articles.Include(x => x.User).Include(x => x.Category)
                .Where(x => x.IsDeleted == false && x.IsActive == true).Skip(arcSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(articles);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}