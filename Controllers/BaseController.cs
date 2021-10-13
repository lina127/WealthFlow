using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WealthFlow.Models;

namespace WealthFlow.Controllers
{
    public class BaseController : Controller
    {
        private WealthflowContext _dbContext;
        public BaseController(WealthflowContext context) 
        {
            _dbContext = context;
        }
        public User GetCurrentUser()
        {
            string email = HttpContext.Session.GetString("CurrentUserEmail");
            if(email != "")
            {
                User user = _dbContext.User.Where(o => o.Email == email).FirstOrDefault();
                return user;
            }
            return null;
        }

        public bool IsSessionValid(out User? user)
        {
            user = GetCurrentUser();
            if(user != null)
            {
                string email = user.Email;
                user = _dbContext.User.Where(o => o.Email == email).FirstOrDefault();
                if (user == null)
                {
                    return false;
                }
                return true;
            }
            user = null;
            return false;
        }

        public void SetUserIdSession(User user)
        {
            HttpContext.Session.SetString("CurrentUserEmail", user.Email);
            HttpContext.Session.SetInt32("CurrentUserId", user.UserId);
        }
    }
}
