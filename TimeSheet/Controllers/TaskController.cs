using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Domain;

namespace Timesheet.Web.Controllers
{
    public class TaskController : Controller
    {
		private Repository<Task> repo = new Repository<Task>();

        //
        // GET: /Task/

        public ActionResult Index()
        {
            return View(repo.AllWith("TaskType").OrderBy(e=>e.TaskType.Name).ThenByDescending(e=>e.CompleteDate).ThenByDescending(e=>e.CreateDate));
        }

		void PopulateBag()
		{
			ViewBag.Types = new SelectList(new Repository<TaskType>().All, "ID", "Name");
		}
        //
        // GET: /Task/Create

        public ActionResult Create()
        {
			PopulateBag();
			return View();
        }

        //
        // POST: /Task/Create

        [HttpPost]
        public ActionResult Create(Task task)
        {
            if (ModelState.IsValid)
            {
                repo.Add(task);
                return RedirectToAction("Index");
            }
			PopulateBag();
            return View(task);
        }

        //
        // GET: /Task/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Task task = repo.FindBy(id);
            if (task == null)
            {
                return HttpNotFound();
            }
			PopulateBag();
            return View(task);
        }

        //
        // POST: /Task/Edit/5

        [HttpPost]
        public ActionResult Edit(Task task)
        {
            if (ModelState.IsValid)
            {
				repo.Update(task);
                return RedirectToAction("Index");
            }
			PopulateBag();
            return View(task);
        }

		public ActionResult Delete(int id)
		{
			repo.Delete(repo.FindBy(id));
			return RedirectToAction("Index");
		}

    }
}