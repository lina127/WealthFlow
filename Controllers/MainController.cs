using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WealthFlow.Controllers
{
    public class MainController : Controller
    {
        private WealthflowContext _dbContext;
        public MainController(WealthflowContext context)
        {
            _dbContext = context;
        }

        public IActionResult Transactions()
        {
            return View();
        }

        public IActionResult Category()
        {
            List<Category> category = _dbContext.Category.ToList();
            return View(category);
        }
        [HttpPost]
        public void RenameCategory(int categoryId, string newName)
        {
            Category category = _dbContext.Category.Where(o => o.CategoryId == categoryId).FirstOrDefault();
            category.Name = newName;
            _dbContext.Update(category);
            _dbContext.SaveChanges();
        }
    }
}
