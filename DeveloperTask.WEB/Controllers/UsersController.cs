using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeveloperTask.DAL;
using DeveloperTask.WEB.Models;

namespace DeveloperTask.Controllers
{
    public class UsersController : Controller
    {
        #region ---- Member Variables ----
        private DeveloperTaskDBEntities m_db = new DeveloperTaskDBEntities();
        #endregion

        #region ----Actions----

        // GET: Users
        public ActionResult Index()
        {
            CheckSessionValid();
            var users = m_db.Users.Where(x => x.Disabled == false);
            ViewBag.ErrorMessage = TempData["UserExistError"];
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(long? id)
        {
            CheckSessionValid();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = m_db.Users.Where(x => x.Disabled == false && x.Id == id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            CheckSessionValid();

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,Email")] User user)
        {
            CheckSessionValid();

            if (ModelState.IsValid)
            {
                var res = ValidateUserWithEmailOrUsernameExist(user);
                if (res is RedirectToRouteResult)
                    return res;

                user.CreateDate = DateTime.UtcNow;
                user.Disabled = false;
                m_db.Users.Add(user);
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(long? id)
        {
            CheckSessionValid();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = m_db.Users.Where(x => x.Disabled == false && x.Id == id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Username,Password,Email,CreateDate")] User user)
        {
            CheckSessionValid();

            if (ModelState.IsValid)
            {
                var res = ValidateUserWithEmailOrUsernameExist(user, true);
                if (res is RedirectToRouteResult)
                    return res;

                user.UpdatedAt = DateTime.UtcNow;
                user.UpdatedBy = CurrentUser.Instance.Id;
                m_db.Entry(user).State = EntityState.Modified;
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(long? id)
        {
            CheckSessionValid();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = m_db.Users.Where(x => x.Disabled == false && x.Id == id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CheckSessionValid();

            User user = m_db.Users.Where(x => x.Disabled == false && x.Id == id).FirstOrDefault();
            //m_db.Users.Remove(user);
            user.Disabled = true;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = CurrentUser.Instance.Id;
            m_db.Entry(user).State = EntityState.Modified;
            m_db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region ---- Methods ----

        /// <summary>
        ///  Check if any User with same username/email already exists
        /// </summary>
        /// <param name="user">User that needs to be matched with DB</param>
        /// <param name="bExcludeCurrentUserId">Match results other then the provided User.</param>
        /// <returns></returns>
        private ActionResult ValidateUserWithEmailOrUsernameExist(User user, bool bExcludeCurrentUserId = false)
        {
            User existedUser = null;
            if (bExcludeCurrentUserId)
                existedUser = m_db.Users.Where(x => x.Id != user.Id && (x.Username.ToLower() == user.Username.ToLower() || x.Email.ToLower() == user.Email.ToLower())).FirstOrDefault();
            else
                existedUser = m_db.Users.Where(x => (x.Username.ToLower() == user.Username.ToLower() || x.Email.ToLower() == user.Email.ToLower())).FirstOrDefault();

            if (existedUser != null)
            {
                TempData["UserExistError"] = "another user with same Username or Email already exists in the system.";
                return RedirectToAction("Index");
            }
            return new EmptyResult();
        }

        /// <summary>
        /// check if user is logged in otherwise redirect to Login
        /// </summary>
        private void CheckSessionValid()
        {
            if (Session["UserID"] == null)
                Response.Redirect("/Login/Index");
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
