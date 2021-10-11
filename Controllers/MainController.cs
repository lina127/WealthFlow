using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WealthFlow.Controllers
{
    public class MainController : Controller
    {
        public IActionResult Transactions()
        {
            return View();
        }
    }
}
