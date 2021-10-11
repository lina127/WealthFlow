using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WealthFlow
{
    public class UserController : Controller
    {
        private WealthflowContext _dbContext;
        public UserController(WealthflowContext context)
        {
            _dbContext = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MyAccount()
        {
            return View();
        }

        [HttpPost]
        public bool IsUserValid(string email, string password)
        {
            bool result = _dbContext.User.Select(o => o.Email == email && o.Password == password).FirstOrDefault();
            return result;
        }
    }
}
