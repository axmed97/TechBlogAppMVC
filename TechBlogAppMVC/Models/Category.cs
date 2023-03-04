using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechBlogAppMVC.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Category boşdur")]
        [MinLength(5)]
        [MaxLength(50)]
        //[Display(Name = "Category Name")]
        public string CategoryName { get; set; }
        public List<Article>? Articles { get; set; }
    }
}
