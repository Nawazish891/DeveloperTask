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
    public class SubCategoriesController : Controller
    {
        #region ---- Member Variables ----
        private DeveloperTaskDBEntities m_db = new DeveloperTaskDBEntities();
        #endregion

        #region ---- Actions ----
        // GET: SubCategories
        public ActionResult Index()
        {
            CheckSessionValid();

            var subCategories = m_db.SubCategories.Include(s => s.Category).Where(x => x.Disabled == false);
            return View(subCategories.ToList());
        }

        // GET: SubCategories/Details/5
        public ActionResult Details(long? id)
        {
            CheckSessionValid();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubCategory subCategory = m_db.SubCategories.Include(x=>x.Reminders).Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id && x.Id == id).FirstOrDefault();
            if (subCategory == null)
            {
                return HttpNotFound();
            }
            return View(subCategory);
        }

        // GET: SubCategories/Create
        public ActionResult Create()
        {
            CheckSessionValid();

            SubCategory subCategory = new SubCategory()
            {
                CatergoryId = Convert.ToInt64(TempData["CategoryId"])
            };
            ViewBag.Catergories = new SelectList(m_db.Categories.Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            return View(subCategory);
        }

        // POST: SubCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,CatergoryId")] SubCategory subCategory)
        {
            CheckSessionValid();

            if (subCategory.CatergoryId == 0)
                return HttpNotFound("Category must be selected to Add Sub Category.");

            if (ModelState.IsValid)
            {
                subCategory.CreateDate = DateTime.UtcNow;
                subCategory.CreatedBy = CurrentUser.Instance.Id;
                m_db.SubCategories.Add(subCategory);
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Catergories = new SelectList(m_db.Categories.Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            return View(subCategory);
        }

        // GET: SubCategories/Edit/5
        public ActionResult Edit(long? id)
        {
            CheckSessionValid();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubCategory subCategory = m_db.SubCategories.Where(x=>x.Disabled == false && x.Id == id).FirstOrDefault();
            if (subCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.Catergories = new SelectList(m_db.Categories.Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            return View(subCategory);
        }

        // POST: SubCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CatergoryId,CreateDate,CreatedBy")] SubCategory subCategory)
        {
            CheckSessionValid();

            if (ModelState.IsValid)
            {
                subCategory.UpdatedAt = DateTime.UtcNow;
                subCategory.UpdatedBy = CurrentUser.Instance.Id;
                m_db.Entry(subCategory).State = EntityState.Modified;
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Catergories = new SelectList(m_db.Categories.Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id), "Id", "Name");
            return View(subCategory);
        }

        // GET: SubCategories/Delete/5
        public ActionResult Delete(long? id)
        {
            CheckSessionValid();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubCategory subCategory = m_db.SubCategories.Where(x => x.Disabled == false && x.Id == id).FirstOrDefault();
            if (subCategory == null)
            {
                return HttpNotFound();
            }
            return View(subCategory);
        }

        // POST: SubCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CheckSessionValid();

            SubCategory subCategory = m_db.SubCategories.Where(x => x.Disabled == false && x.Id == id).FirstOrDefault();
            subCategory.UpdatedAt = DateTime.UtcNow;
            subCategory.UpdatedBy = CurrentUser.Instance.Id;
            subCategory.Disabled = true;
            m_db.Entry(subCategory).State = EntityState.Modified;
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
