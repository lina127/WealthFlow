using Microsoft.AspNetCore.Http;
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
            if (IsSessionValid(out User user))
            {
                return View(user);
            }
            return RedirectToAction("Index");
            
        }

        public User GetCurrentUser()
        {
            string email = HttpContext.Session.GetString("CurrentUserEmail");
            User user = _dbContext.User.Where(o => o.Email == email).FirstOrDefault();
            return user;
        }

        public bool IsSessionValid(out User user)
        {
            string email = HttpContext.Session.GetString("CurrentUserEmail");

            user = _dbContext.User.Where(o => o.Email == email).FirstOrDefault();
            if (user == null)
            {
                return false;
            }
            return true;
        }

        [HttpPost]
        public bool IsUserValid(string email, string password)
        {
            bool result = _dbContext.User.Select(o => o.Email == email && o.Password == password).FirstOrDefault();
            if (result)
            {
                HttpContext.Session.SetString("CurrentUserEmail", email);
            }
            return result;
        }

        [HttpPost]
        public bool UpdateAccount(string email, string password)
        {
            email = email.Trim();
            password = password.Trim();
            string oldEmail = HttpContext.Session.GetString("CurrentUserEmail");
            
            User user = _dbContext.User.Where(o => o.Email == oldEmail).FirstOrDefault();

            if(user.Email.Trim() == email && user.Password.Trim() == password)
                return false;
            user.Email = email;
            user.Password = password;
            _dbContext.User.Update(user);
            _dbContext.SaveChanges();

            return true;
        }

        [HttpPost]
        public void AddNewCard(int cardNum, string type)
        {
            User user = GetCurrentUser();
            Card card = new Card();
            card.CardNum = cardNum;
            card.Type = type;
            card.UserId = user.UserId;
            _dbContext.Card.Add(card);
            _dbContext.SaveChanges();
        }
    }
}
