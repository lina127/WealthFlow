using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WealthFlow.Controllers
{
    public class MainController : BaseController
    {
        private WealthflowContext _dbContext;
        public MainController(WealthflowContext context) : base(context)
        {
            _dbContext = context;
        }

        public IActionResult Transactions() 
        {
            if (IsSessionValid(out User? user))
            {
                List<Card> cards = _dbContext.Card.Where(o => o.UserId == user.UserId).ToList();
                
                List<Transaction> transactions = _dbContext.Transaction.Where(o => o.Card.UserId == user.UserId).ToList();

                DataDTO dataDTO = new(user, cards, transactions);
                return View(dataDTO);
            }
            return RedirectToAction("Index", "User");
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
