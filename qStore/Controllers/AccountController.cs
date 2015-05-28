using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using qStore.Filters;
using qStore.Models;

namespace qStore.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult LogIn(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            return View(new LoginModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SendLogIn(LoginModel model)
        {
            if (Authorize(model))
            {
                return Redirect(model.ReturnUrl);
            }
            else
            {
                ModelState.AddModelError("Name", "Incorrect User Name or Password");
                return View("LogIn", model);
            }
        }

        public ActionResult LogOut()
        {
            //Clear user cookie
            var cookie = new HttpCookie("credentials", null);
            cookie.Expires = DateTime.Now.AddDays(-1);

            Response.Cookies.Add(cookie);

            return RedirectToAction("Index", "Home");
        }

        private bool Authorize(LoginModel model)
        {
            //Check if user is in DB and write into cookie
            if (!string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.Password))
            {
                string hash;
                using (MD5 md5Hash = MD5.Create())
                {
                    hash = GetMd5Hash(md5Hash, model.Password);
                }

                using(var db = new qStoreDBEntities()){
                    User user = db.Users.SingleOrDefault(x => x.Email == model.Name);

                    if (user != null && user.PassHash == hash) 
                    {
                        var cookie = new HttpCookie("credentials", model.Name);
                        cookie.Expires = DateTime.Now.AddDays(2);

                        Response.Cookies.Add(cookie);

                        return true;
                    }
                }

            }

            return false;
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}