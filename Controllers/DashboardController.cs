using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WealthFlow.Models;

namespace WealthFlow.Controllers
{
    public class DashboardController : BaseController
    {
        private WealthflowContext _dbContext;
        public DashboardController(WealthflowContext context) : base(context)
        {
            _dbContext = context;
        }
        public IActionResult Index()
        {
            if (IsSessionValid(out User? user))
            {
                return View();
            }
            return RedirectToAction("Index", "User");
        }

        [HttpPost]
        public JsonResult GetTotalAmountPerCategory(DateTime from, DateTime to, int dataType)
        {
            User user = GetCurrentUser();

            // Total amount per category
            if (dataType == 1)
            {
                List<DashboardDTO> data = _dbContext.Transaction.Where(o => o.Card.UserId == user.UserId && o.Date >= from && o.Date <= to).GroupBy(o => o.Category.Name).Select(o => new DashboardDTO { Category = o.Key ?? "Uncategorized", Amount = o.Sum(x => x.Amount) }).ToList();

                return Json(data);
            }

            // Total income per category
            else if (dataType == 2)
            {
                List<DashboardDTO> data = _dbContext.Transaction.Where(o => o.Card.UserId == user.UserId && o.Amount > 0 && o.Date >= from && o.Date <= to).GroupBy(o => o.Category.Name).Select(o => new DashboardDTO { Category = o.Key ?? "Uncategorized", Amount = o.Sum(x => x.Amount) }).ToList();

                return Json(data);
            }

            // Total expense per category
            else if (dataType == 3)
            {
                List<DashboardDTO> data = _dbContext.Transaction.Where(o => o.Card.UserId == user.UserId && o.Amount < 0 && o.Date >= from && o.Date <= to).GroupBy(o => o.Category.Name).Select(o => new DashboardDTO { Category = o.Key ?? "Uncategorized", Amount = o.Sum(x => (x.Amount * -1)) }).ToList();

                return Json(data);
            }
            return null;
        }


        [HttpPost]
        public JsonResult GetTotalAmountPerMonth(DateTime from, DateTime to, int dataType)
        {
            User user = GetCurrentUser();

            // Total amount per month
            if(dataType == 1)
            {
                List<DashboardDTO> data = _dbContext.Transaction.Where(o => o.Card.UserId == user.UserId && o.Date >= from && o.Date <= to).GroupBy(o => new { Month = o.Date.Month, Year = o.Date.Year }).Select(o => new DashboardDTO { Month = o.Key.Month, Year = o.Key.Year, Amount = o.Sum(x => x.Amount) }).ToList();

                return Json(data);
            }

            // Total income per month
            else if (dataType == 2)
            {
                List<DashboardDTO> data = _dbContext.Transaction.Where(o => o.Card.UserId == user.UserId && o.Amount > 0 && o.Date >= from && o.Date <= to).GroupBy(o => new { Month = o.Date.Month, Year = o.Date.Year }).Select(o => new DashboardDTO { Month = o.Key.Month, Year = o.Key.Year, Amount = o.Sum(x => x.Amount) }).ToList();

                return Json(data);
            }
            
            // Total expense per month
            else if(dataType == 3)
            {
                List<DashboardDTO> data = _dbContext.Transaction.Where(o => o.Card.UserId == user.UserId && o.Amount < 0 && o.Date >= from && o.Date <= to).GroupBy(o => new { Month = o.Date.Month, Year = o.Date.Year }).Select(o => new DashboardDTO { Month = o.Key.Month, Year = o.Key.Year, Amount = o.Sum(x => (x.Amount * -1)) }).ToList();

                return Json(data);
            }
            return null;
        }

        [HttpPost]
        public JsonResult GetRecentTransactions(int total)
        {
            User user = GetCurrentUser();
            List<Transaction> transactions = _dbContext.Transaction.Where(o => o.Card.UserId == user.UserId).OrderByDescending(o => o.Date).Take(total).ToList();
            List<DashboardDTO> data = new();
            foreach(var t in transactions)
            {
                DashboardDTO d = new();
                d.Amount = t.Amount;
                d.Merchant = t.Merchant;
                d.Date = t.Date;
                data.Add(d);
            }
            return Json(data);
        }
    }
}
