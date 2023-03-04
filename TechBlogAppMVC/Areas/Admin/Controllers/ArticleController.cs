using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechBlogAppMVC.Data;
using TechBlogAppMVC.Helpers;
using TechBlogAppMVC.Models;

namespace TechBlogAppMVC.Areas.Admin.Controllers
{
    [Area(nameof(Admin))]
    [Authorize(Roles = "Admin, Admin Editor, Editor")]
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IWebHostEnvironment _env;
        public ArticleController(AppDbContext context, IHttpContextAccessor contextAccessor, IWebHostEnvironment env)
        {
            _context = context;
            _contextAccessor = contextAccessor;
            _env = env;
        }

        public IActionResult Index()
        {
            var articles = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.User)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag).Where(x => x.IsDeleted == false).ToList();
            return View(articles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewData["Tags"] = tags;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Article article, List<int> Tags, IFormFile Photo)
        {
            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewData["Tags"] = tags;

            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            article.UserId = userId;

            article.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);

            article.SeoUrl = article.Title.ReplaceInvalidChars();

            article.CreatedDate = DateTime.Now;
            article.UpdatedDate = DateTime.Now;

            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();


            List<ArticleTag> newArticleTag = new();

            for (int i = 0; i < Tags.Count; i++)
            {
                ArticleTag articleTag = new()
                {
                    ArticleId = article.Id,
                    TagId = Tags[i] 
                };
                newArticleTag.Add(articleTag);
            }

            await _context.ArticleTags.AddRangeAsync(newArticleTag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();


            var article = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .FirstOrDefault(x => x.Id == id);

            if (article == null) return NotFound();

            var categories = _context.Categories.ToList();
            var tags = _context.Tags.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "CategoryName");
            ViewData["Tags"] = tags;

            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Article article, List<int> Tags, IFormFile Photo)
        {
            article.UpdatedDate = DateTime.Now;

            if(Photo != null)
            {
                article.PhotoUrl = await Photo.SaveFileAsync(_env.WebRootPath);
            }

            var oldTags = _context.ArticleTags.Where(x => x.ArticleId == article.Id).ToList();

            _context.ArticleTags.RemoveRange(oldTags);
            await _context.SaveChangesAsync();

            List<ArticleTag> newArticleTag = new();

            for (int i = 0; i < Tags.Count; i++)
            {
                ArticleTag articleTag = new()
                {
                    ArticleId = article.Id,
                    TagId = Tags[i]
                };
                newArticleTag.Add(articleTag);
            }

            await _context.ArticleTags.AddRangeAsync(newArticleTag);
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
