using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using WebAPI.Models;
using WebAPI.Models.UserModels;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Authorization")]
    public class AuthorizationController : Controller
    {
        [HttpGet("/api/Authorization/check", Name = "CheckAuthorization", Order = 1)]
        public bool Get()
        {
            if (HttpContext.Session.GetInt32("authorizated") == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpGet]
        public UserToReturn Get(string login, string password)
        {
            DbContext _user_context = HttpContext.RequestServices.GetService(typeof(DbContext)) as DbContext;
            UserFromDbModel _user = _user_context.GetCurrentUser(login);
            UserToReturn result = new UserToReturn();

            if (_user == null)
            {
                HttpContext.Session.Clear();
            }
            else if (_user.upassword == password)
            {
                HttpContext.Session.SetInt32("authorizated", 1);
                if (_user.administrator)
                    HttpContext.Session.SetInt32("administrator", 1);
                result.authorizated = true;
                if (_user.administrator)
                    result.administrator = true;
                if (_user.accounter)
                    result.accounter = true;
                if (_user.customer)
                    result.customer = true;
                if (_user.realizator)
                    result.realizator = true;
            }
            else
            {
                HttpContext.Session.Clear();
            }            

            return result;
        }
    }
}