using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DeveloperTask.DAL;

namespace DeveloperTask.Controllers
{
    public class UsersController : Controller
    {
        private DeveloperTaskDBEntities m_db = new DeveloperTaskDBEntities();

        // GET: Users
        public ActionResult Index()
        {
            var users = m_db.Users.Where(x => x.Disabled == false);
            ViewBag.ErrorMessage = TempData["UserExistError"];
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(long? id)
        {
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
            ViewBag.UpdatedBy = new SelectList(m_db.Users, "Id", "Username");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Username,Password,Email")] User user)
        {
            if (ModelState.IsValid)
            {
                var res = CheckUserWithEmailOrUsernameExist(user);
                if (res is RedirectToRouteResult)
                    return res;

                user.CreateDate = DateTime.UtcNow;
                user.Disabled = false;
                m_db.Users.Add(user);
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UpdatedBy = new SelectList(m_db.Users, "Id", "Username", user.UpdatedBy);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = m_db.Users.Where(x => x.Disabled == false && x.Id == id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UpdatedBy = new SelectList(m_db.Users, "Id", "Username", user.UpdatedBy);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Username,Password,Email,CreateDate")] User user)
        {
            if (ModelState.IsValid)
            {
                var res = CheckUserWithEmailOrUsernameExist(user, true);
                if (res is RedirectToRouteResult)
                    return res;

                user.UpdatedAt = DateTime.UtcNow;
                m_db.Entry(user).State = EntityState.Modified;
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UpdatedBy = new SelectList(m_db.Users, "Id", "Username", user.UpdatedBy);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(long? id)
        {
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
            User user = m_db.Users.Where(x => x.Disabled == false && x.Id == id).FirstOrDefault();
            //m_db.Users.Remove(user);
            user.Disabled = true;
            user.UpdatedAt = DateTime.UtcNow;
            m_db.Entry(user).State = EntityState.Modified;
            m_db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Check if any User with same username/email already exists
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private ActionResult CheckUserWithEmailOrUsernameExist(User user, bool bFromUpdate = false)
        {
            User existedUser = null;
            if (bFromUpdate)
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
    }
}
