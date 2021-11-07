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
        private DeveloperTaskDBEntities db = new DeveloperTaskDBEntities();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Where(x=>x.Disabled == false);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Where(x=>x.Disabled == false && x.Id == id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "Username");
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
                user.CreateDate = DateTime.UtcNow;
                user.Disabled = false;
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "Username", user.UpdatedBy);
            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Where(x => x.Disabled == false && x.Id == id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "Username", user.UpdatedBy);
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
                user.UpdatedAt = DateTime.UtcNow;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UpdatedBy = new SelectList(db.Users, "Id", "Username", user.UpdatedBy);
            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Where(x => x.Disabled == false && x.Id == id).FirstOrDefault();
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
            User user = db.Users.Where(x => x.Disabled == false && x.Id == id).FirstOrDefault();
            //db.Users.Remove(user);
            user.Disabled = true;
            user.UpdatedAt = DateTime.UtcNow;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Disposing DB Context
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
