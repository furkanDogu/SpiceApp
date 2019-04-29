using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpiceApp.Models.Entities;
using SpiceApp.DataAccessLayer.Concretes;
using SpiceApp.BusinessLayer.Validation;
using System.Threading.Tasks;
using BCrypt.Net;

namespace SpiceApp.BusinessLayer.Concretes
{
    public class UserBusiness : IDisposable
    {
        public Response<User> RegisterUser(User entity)
        {
            Response<User> res = new Response<User>();
            try
            {
                if (!UserValidation.CheckIfUsernameExists(entity.Username, res))
                {
                    // hash the user password before it's saved in the db.
                    entity.Password = BCrypt.Net.BCrypt.HashPassword(entity.Password);
                    try
                    {
                        using (var repo = new UserRepository())
                        {
                            repo.Insert(entity);
                        }
                        res.Message = "Kayıt işlemi başarı ile gerçekleştirildi";
                        res.isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("An error occured while executing RegisterUser() in SpiceApp.BusinessLayer.UserBusiness", ex);
                    }
                }
                else
                {
                    res.isSuccess = false;
                    res.Message = "Kullanıcı ismi bir başkası tarafından kullanılıyor";
                }

                return res;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured in RegisterUser() func in SpiceApp.BusinessLayer.UserBusiness",ex);
            }
        }

        public Response<User> Login(string Username, string Password)
        {
            Response<User> res = new Response<User>();

            try
            {
                using (var repo = new UserRepository())
                {
                    var user = repo.FetchByUsername(Username);
                    if (user != null)
                    {
                        if (BCrypt.Net.BCrypt.Verify(Password, user.Password)) // compare encrypted password with the given one. if matched, return matched user info. 
                        {
                            res.Message = "Sisteme giriş başarılı";
                            res.isSuccess = true;
                            res.Data = new List<User>();
                            user.Password = "";
                            res.Data.Add(user);
                        }
                        else
                        {
                            res.Data = null;
                            res.isSuccess = false;
                        }
                    }
                }

                if (!res.isSuccess)
                {
                    res.Message = "Lütfen girdiğiniz bilgileri kontrol ediniz";
                }

                return res;
            }
            catch(Exception ex)
            {
                throw new Exception("An error occured in Login() func. in SpiceApp.BusinessLayer.UserBusiness", ex);
            }
        }

        public Response<User> UpdateInfo(User entity)
        {
            Response<User> res = new Response<User>();
            try
            {
                using(var repo = new UserRepository())
                {
                    res.isSuccess = repo.Update(entity);
                    if (res.isSuccess)
                        res.Message = "Kullanıcı bilgileriniz başarıyla güncellendi";
                    else
                        res.Message = "Kullanıcı bilgileri güncellenirken bir sorun ile karşılaşıldı";
                }
                return res;

            }
            catch (Exception ex)
            { 
                throw new Exception("An error occured in UpdateInfo() function in SpiceApp.BusinessLayer.UserBusiness" ,ex);
            }
        }

        public Response<User> FetchAllCustomers()
        {
            Response<User> res = new Response<User>();
            try
            {
                using(var repo = new UserRepository())
                {
                    res.Data = repo.FetchAllCustomers();
                    res.isSuccess = true;
                    res.Message = "Müşterilerin listesi";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in FetchAllCustomers() in SpiceApp.BusinessLayer.UserBusiness",ex);
            }
        }

        public Response<User> ChangePassword(int UserID, string NewPassword)
        {
            Response<User> res = new Response<User>();
            NewPassword = BCrypt.Net.BCrypt.HashPassword(NewPassword);
            try
            {
                using(var repo = new UserRepository())
                {
                    res.isSuccess = repo.ChangePassword(UserID, NewPassword);

                    if (res.isSuccess)
                        res.Message = "Şifre başarı ile değiştirildi";
                    else
                        res.Message = "Şifre değiştirilirken bir hata meydana geldi";
                }
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in SpiceApp.BusinessLayer.UserBusiness", ex);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
