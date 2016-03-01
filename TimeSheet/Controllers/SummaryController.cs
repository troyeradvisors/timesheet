using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hub.Domain.Repositories;
using Hub.Domain.Entities;
using Timesheet.Web.Models;

namespace Timesheet.Web.Controllers
{
    public class SummaryController : Controller
    {
        public ActionResult Index()
        {
			var model = new SummaryModel();
			model.UpdateSummaries();
            return View(model);
        }

		[HttpPost]
		public ActionResult Index(SummaryModel model)
		{
			model.UpdateSummaries();
			return View(model);
		}

    }
}
