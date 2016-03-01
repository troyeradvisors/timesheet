using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Domain;
using Timesheet.Web.Models;
using MvcBase.Infrastructure;
using System.Web.Security;
namespace TimeSheet.Controllers
{
    public class LogController : Controller
    {
		Repository<Log> repo = new Repository<Log>();
		int MaxDisplayWeeks = 4;

        public ActionResult Index()
        {
			var models = LogModel.RecentWeeks(MaxDisplayWeeks, repo).ToList();
			Session["models"] = models;
            return View(models);
        }

		[HttpPost]
		public ActionResult Index(double[,,] hour, int[,] task)
		{
			Guid userID = Hub.Domain.Entities.User.Current.ID;
			var start = DateTime.Today.AddDays(7-((int)DateTime.Today.DayOfWeek) + 1 - 7*MaxDisplayWeeks);
			var end = start.AddDays(7*MaxDisplayWeeks);
			repo.All.Where(e => e.UserID == userID && e.Date >= start && e.Date < end).ToList().ForEach(e => repo.Delete(e, false));

			for (int i=0; i<hour.GetLength(0); ++i)
				for (int j=0; j<hour.GetLength(1); ++j)
					for (int k = 0; k < hour.GetLength(2); ++k)
					{
						Log log = new Log 
						{ 
							UserID = userID,
							TaskID = task[i,j],
							Date = start.AddDays(7*(MaxDisplayWeeks-1-i) + k),
							Hours = hour[i, j, k]
						};
						if (log.Hours > 0 && log.TaskID > 0)
							repo.Add(log, false);
					}
			repo.Save();
			return RedirectToAction("Index");
		}

		[ChildActionOnly]
		public ActionResult Calculator()
		{
			Calculator calc = (Calculator) Session["Calculator"];
			calc = calc ?? new Calculator();
			return PartialView(calc);
		}

		[HttpPost]
		public ActionResult Calculator(Calculator calc)
		{
			List<int> rows = new List<int>();

			for (int i = 0; i < calc.Times.GetLength(0); ++i)
			{
				if (calc.Times[i, 0] != DateTime.MinValue|| calc.Times[i, 1] != DateTime.MinValue)
					rows.Add(i);
			}
			DateTime[,] times = new DateTime[rows.Count, 2];
			for (int i=0; i<rows.Count(); ++i)
			{
				times[i, 0] = calc.Times[rows[i], 0];
				times[i, 1] = calc.Times[rows[i], 1];
			}
			calc.Times = times;
			Session["Calculator"] = calc;
			return PartialView(calc);
		}

    }
}