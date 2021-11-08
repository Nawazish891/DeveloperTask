using DeveloperTask.DAL;
using DeveloperTask.WEB.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DeveloperTask.WEB.Controllers
{
    public class LoginController : Controller
    {
        #region ---- Member Variables ----
        private DeveloperTaskDBEntities m_db = new DeveloperTaskDBEntities();
        #endregion
        #region ---- Actions ----
        // GET: Login
        public ActionResult Index()
        {
            ViewBag.Error = TempData["ErrorMessage"];
            return View();
        }

        [HttpPost]
        public ActionResult Authorize(LoginUserModel user)
        {
            if(ModelState.IsValid)
            {
                User userDetails = m_db.Users.Where(x => x.Disabled == false && x.Username == user.Username && x.Password == user.Password).FirstOrDefault();

                if(userDetails == null)
                {
                    TempData["ErrorMessage"] = "Username or password is incorrect.";
                    return RedirectToAction("Index", "Login");
                }

                CurrentUser.Instance.Id = userDetails.Id;
                Session["UserID"] = userDetails.Id;
                user.Email = userDetails.Email;
                EncryptUserInCookie(user);
                return RedirectToAction("Index","Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Username and password is required.";
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult Logout()
        {
            Session["UserID"] = null;
            CurrentUser.Instance.Id = 0;
            return RedirectToAction("Index");
        }
        #endregion

        #region ---- Methods ----
        /// <summary>
        /// Encrypt User Details in Cookie
        /// </summary>
        /// <param name="userDetails"></param>
        private void EncryptUserInCookie(LoginUserModel userDetails)
        {
            var jsonObject = JsonConvert.SerializeObject(userDetails);
            var cookieText = Encoding.UTF8.GetBytes(jsonObject);
            var encryptedValue = Convert.ToBase64String(MachineKey.Protect(cookieText, "ProtectCookie"));
            //--- Create cookie object and pass name of the cookie and value to be stored.
            HttpCookie cookieObject = new HttpCookie("UserCookie", encryptedValue);
            //---- Set expiry time of cookie.
            cookieObject.Expires.AddDays(5);
            //---- Add cookie to cookie collection.
            Response.Cookies.Add(cookieObject);
        }
        #endregion

        #region ---- Dispose ----

        /// <summary>
        /// Disposing DB Context
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
