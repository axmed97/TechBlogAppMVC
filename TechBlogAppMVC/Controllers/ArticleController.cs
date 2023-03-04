using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TechBlogAppMVC.Data;
using TechBlogAppMVC.Models;
using TechBlogAppMVC.ViewModel;

namespace TechBlogAppMVC.Controllers
{
    public class ArticleController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public ArticleController(AppDbContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Detail(int? id)
        {
            if (id == null) return NotFound();

            var article = _context.Articles
                .Include(x => x.Category)
                .Include(x => x.ArticleTags)
                .ThenInclude(x => x.Tag)
                .Include(x =>x.User)
                .Include(x => x.Comments)
                .FirstOrDefault(x => x.Id == id);

            if(article == null) return NotFound();


            var cookie = _contextAccessor.HttpContext.Request.Cookies[$"Views"];
            string[] findCookie = { "" };


            if (cookie != null)
            {
                findCookie = cookie.Split('-').ToArray();
            }


            if (!findCookie.Contains(article.Id.ToString()))
            {
                Response.Cookies.Append($"Views", $"{cookie}-{article.Id}",
                    new CookieOptions
                    {
                        Secure = true,
                        HttpOnly = true,
                        Expires = DateTime.Now.AddYears(30),
                    });

                article.ViewCount += 1;
                _context.Articles.Update(article);
                _context.SaveChanges();
            }
            var before = _context.Articles.FirstOrDefault(x => x.Id < id);
            var after = _context.Articles.FirstOrDefault(x => x.Id > id);
            var suggestion = _context.Articles.Include(x => x.Category).Where(x => x.CategoryId == article.CategoryId).Take(2).ToList();
            DetailVM detail = new()
            {
                Article = article,
                Suggestion = suggestion,
                After = after,
                Before = before
            };

            return View(detail);
        }

        [HttpPost]
        public async Task<IActionResult> Detail(Comment comment)
        {
            var userId = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            Comment newComment = new();
            newComment.UserId = userId;
            newComment.CreatedDate = DateTime.Now;
            newComment.Content = comment.Content;
            newComment.ArticleId = comment.ArticleId;

            var article = _context.Articles.FirstOrDefault(x => x.Id == comment.ArticleId);
            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();


            return RedirectToAction(nameof(Detail), new {id = article.Id, article.SeoUrl});
        }
    }
}
