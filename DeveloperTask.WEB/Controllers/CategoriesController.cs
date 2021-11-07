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
    public class CategoriesController : Controller
    {
        #region ---- Member Variables ----
        private DeveloperTaskDBEntities m_db = new DeveloperTaskDBEntities();
        #endregion

        #region ---- Actions ----
        // GET: Categories
        public ActionResult Index()
        {
            var categories = m_db.Categories.Where(x=>x.Disabled == false && x.CreatedBy == CurrentUser.Instance.Id);
            return View(categories.ToList());
        }

        // GET: Categories/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = m_db.Categories.Include(x=>x.Reminders).Include(x=>x.SubCategories).Where(x => x.Disabled == false && x.Id == id && x.CreatedBy == CurrentUser.Instance.Id).FirstOrDefault();
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedBy = CurrentUser.Instance.Id;
                category.CreateDate = DateTime.UtcNow;
                m_db.Categories.Add(category);
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = m_db.Categories.Where(x => x.Disabled == false && x.Id == id && x.CreatedBy == CurrentUser.Instance.Id).FirstOrDefault();
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CreateDate,CreatedBy")] Category category)
        {
            if (ModelState.IsValid)
            {
                category.UpdatedAt = DateTime.UtcNow;
                category.UpdatedBy= CurrentUser.Instance.Id;

                m_db.Entry(category).State = EntityState.Modified;
                m_db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = m_db.Categories.Include(x=>x.SubCategories).Where(x => x.Disabled == false && x.Id == id && x.CreatedBy == CurrentUser.Instance.Id).FirstOrDefault();

            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Category category = m_db.Categories.Include(x=>x.SubCategories).Where(x => x.Disabled == false && x.Id == id && x.CreatedBy == CurrentUser.Instance.Id).FirstOrDefault();
            category.UpdatedAt = DateTime.UtcNow;
            category.UpdatedBy = CurrentUser.Instance.Id;
            category.Disabled = true;
            m_db.Entry(category).State = EntityState.Modified;
            
            if(category.SubCategories.Any())
            {
                foreach(SubCategory subCat in category.SubCategories)
                {
                    subCat.UpdatedAt = DateTime.UtcNow;
                    subCat.UpdatedBy = CurrentUser.Instance.Id;
                    subCat.Disabled = true;
                    m_db.Entry(subCat).State = EntityState.Modified;
                }
            }

            m_db.SaveChanges();
            return RedirectToAction("Index");
        }


        // POST: Categories/CreateSubCategory/5
        public ActionResult CreateSubCategory(long? id)
        {
            TempData["CategoryId"] = id;
            return RedirectToAction("Create", "SubCategories");
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
