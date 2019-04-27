using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpiceApp.DataAccessLayer.Concretes;
using SpiceApp.Models.Entities;

namespace SpiceApp.BusinessLayer.Validation
{
    public static class UserValidation
    {   
        public static bool CheckIfUsernameExists(string Username, Response<User> res)
        {
            using(var userRepo = new UserRepository())
            {
                var user = userRepo.FetchByUsername(Username);
                if (user == null)
                {
                    return false;
                }
                res.Message = "Bu nickname kullanılıyor !";
                return true;
            }
        }
    }
}
