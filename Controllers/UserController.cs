using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WealthFlow.Models;

namespace WealthFlow.Controllers
{
    public class UserController : BaseController
    {
        private WealthflowContext _dbContext;
        public UserController(WealthflowContext context) : base(context)
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
                List<Card> cards = _dbContext.Card.Where(o => o.UserId == user.UserId).ToList();
                DataDTO dataDTO = new DataDTO(user, cards);
                return View(dataDTO);
            }
            return RedirectToAction("Index");
            
        }

        [HttpPost]
        public bool IsUserValid(string email, string password)
        {
            User result = _dbContext.User.Where(o => o.Email == email && o.Password == password).FirstOrDefault();
            if (result != null)
            {
                SetUserIdSession(result);
                return true;
            }
            return false;
        }

        [HttpPost]
        public bool UpdateAccount(string email, string password)
        {
            email = email.Trim();
            password = password.Trim();
            string oldEmail = GetCurrentUser().Email;

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
        public void AddNewCard(string cardNum, string type, string bank)
        {
            User user = GetCurrentUser();
            Card card = new Card();
            card.CardNum = cardNum;
            card.Type = type;
            card.Bank = bank;
            card.UserId = user.UserId;
            _dbContext.Card.Add(card);
            _dbContext.SaveChanges();
        }

        [HttpPost]
        public void DeleteCard(int cardId)
        {
            Card card = _dbContext.Card.Where(o => o.CardId == cardId).FirstOrDefault();
            _dbContext.Remove(card);
            _dbContext.SaveChanges();
        }
    }
}
