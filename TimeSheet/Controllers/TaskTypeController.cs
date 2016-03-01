using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Domain;

namespace Timesheet.Web.Controllers
{
	public class TaskTypeController : Controller
	{
		private Repository<TaskType> repo = new Repository<TaskType>();

		//
		// GET: /Task/

		public ActionResult Index()
		{
			return View(repo.All.OrderBy(e => e.Name));
		}

		//
		// GET: /TaskType/Create

		public ActionResult Create()
		{
			return View();
		}
		[HttpPost]
		public ActionResult Create(TaskType type)
		{
			if (ModelState.IsValid)
			{
				repo.Add(type);
				return RedirectToAction("Index");
			}

			return View(type);
		}

		//
		// GET: /Task/Edit/5

		public ActionResult Edit(int id = 0)
		{
			TaskType type = repo.FindBy(id);
			if (type == null)
			{
				return HttpNotFound();
			}
			return View(type);
		}

		//
		// POST: /Task/Edit/5

		[HttpPost]
		public ActionResult Edit(TaskType type)
		{
			if (ModelState.IsValid)
			{
				repo.Update(type);
				return RedirectToAction("Index");
			}
			return View(type);
		}

		public ActionResult Delete(int id)
		{
			repo.Delete(repo.FindBy(id));
			return RedirectToAction("Index");
		}
	}
}
