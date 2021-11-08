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

namespace DeveloperTask.WEB.Controllers
{
    public class RemindersController : Controller
    {
        #region ---- Member Variables ----
        private DeveloperTaskDBEntities m_db = new DeveloperTaskDBEntities();
        #endregion

        #region ---- Actions ---- 
        // GET: Reminders
        public ActionResult Index()
        {
            CheckSessionValid();
            var reminders = m_db.Reminders.Include(r => r.Category).Include(r => r.SubCategory).Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id);
            return View(reminders.ToList());
        }

        // GET: Reminders/Details/5
        public ActionResult Details(long? id)
        {
            CheckSessionValid();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reminder reminder = m_db.Reminders.Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id && x.Id == id).FirstOrDefault();
            if (reminder == null)
            {
                return HttpNotFound();
            }
            return View(reminder);
        }

        // GET: Reminders/Create
        public ActionResult Create()
        {
            CheckSessionValid();
            ViewBag.Catergories = new SelectList(m_db.Categories.Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            ViewBag.SubCatergories = new SelectList(m_db.SubCategories.Where(x => x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            return View();
        }

        // POST: Reminders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Date,CategoryId,SubCategoryId")] Reminder reminder)
        {
            CheckSessionValid();
            if (ModelState.IsValid)
            {
                reminder.CreatedBy = CurrentUser.Instance.Id;
                reminder.CreateDate = DateTime.UtcNow;
                m_db.Reminders.Add(reminder);
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Catergories = new SelectList(m_db.Categories.Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            ViewBag.SubCatergories = new SelectList(m_db.SubCategories.Where(x => x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            return View(reminder);
        }

        // GET: Reminders/Edit/5
        public ActionResult Edit(long? id)
        {
            CheckSessionValid();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reminder reminder = m_db.Reminders.Where(x => x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id && x.Id == id).FirstOrDefault();

            if (reminder == null)
            {
                return HttpNotFound();
            }
            ViewBag.Catergories = new SelectList(m_db.Categories.Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            ViewBag.SubCatergories = new SelectList(m_db.SubCategories.Where(x => x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            return View(reminder);
        }

        // POST: Reminders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Date,CategoryId,SubCategoryId,CreatedBy,CreateDate")] Reminder reminder)
        {
            CheckSessionValid();
            if (ModelState.IsValid)
            {
                reminder.UpdatedAt = DateTime.UtcNow;
                reminder.UpdatedBy = CurrentUser.Instance.Id;
                m_db.Entry(reminder).State = EntityState.Modified;
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Catergories = new SelectList(m_db.Categories.Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            ViewBag.SubCatergories = new SelectList(m_db.SubCategories.Where(x => x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            return View(reminder);
        }

        // GET: Reminders/Delete/5
        public ActionResult Delete(long? id)
        {
            CheckSessionValid();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reminder reminder = m_db.Reminders.Where(x => x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id && x.Id == id).FirstOrDefault();
            if (reminder == null)
            {
                return HttpNotFound();
            }
            return View(reminder);
        }

        // POST: Reminders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CheckSessionValid();
            Reminder reminder = m_db.Reminders.Where(x => x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id && x.Id == id).FirstOrDefault();
            reminder.UpdatedAt = DateTime.UtcNow;
            reminder.UpdatedBy = CurrentUser.Instance.Id;
            reminder.Disabled = true;
            m_db.Entry(reminder).State = EntityState.Modified;
            m_db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion

        #region ---- Methods ----

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
