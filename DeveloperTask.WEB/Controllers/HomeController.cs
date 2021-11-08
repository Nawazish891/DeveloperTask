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

namespace DeveloperTask.Controllers
{
    public class HomeController : Controller
    {
        #region ---- Actions ----
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                Response.Redirect("/Login/Index");
            else
                DecryptUserCookie();
            return View();
        }
        #endregion

        #region ---- Methods ----
        /// <summary>
        /// Decrypt User Cookie and Store in Static Variable to show in Home Page
        /// </summary>
        private void DecryptUserCookie()
        {
            var bytes = Convert.FromBase64String(Request.Cookies["UserCookie"]?.Value);
            var output = MachineKey.Unprotect(bytes, "ProtectCookie");
            string result = Encoding.UTF8.GetString(output);
            try
            {
                LoginUserModel user = JsonConvert.DeserializeObject<LoginUserModel>(result);
                if (user != null)
                {
                    CurrentUser.Instance.Username = user.Username;
                    CurrentUser.Instance.Email = user.Email;
                }
            }
            catch (Exception ex)
            {
                //Log Here
            }
        }
        #endregion

    }
}