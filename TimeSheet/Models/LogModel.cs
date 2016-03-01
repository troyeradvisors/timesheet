using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Timesheet.Domain;
using System.Web.Mvc;
using Hub.Domain.Entities;
namespace Timesheet.Web.Models
{

	public class LogModel
	{
		Repository<Task> tasks = new Repository<Task>();

		public Task[] AvailableTasks { get; set; }
		public Task[] ActualTasks { get; set; }
		public Log[,] Logs { get; set; }
		public DateTime Monday { get; set; }

		public LogModel(DateTime? week, Repository<Log> logs)
		{
			Guid userID = User.Current.ID;
			Monday = GetMonday(week ?? DateTime.Now);
			var end = Monday.AddDays(7);
			var list =  logs.AllWith("Task").Where(e => e.UserID == userID && e.Date >= Monday && e.Date < end).OrderBy(e => e.Task.Name).ThenBy(e => e.Date).ToList();

			int rows = list.Select(e => e.TaskID).Distinct().Count();
			Logs = new Log[rows, 7];
			ActualTasks = new Task[rows];
			int id=-1, i=-1, j=0;
			foreach (Log l in list)
			{
				if (l.TaskID != id)
				{
					id = l.TaskID;
					++i;
					ActualTasks[i] = tasks.FindBy(l.TaskID);
				}
				j = (((int)l.Date.DayOfWeek) + 6) % 7; // Make Monday the first day of the week!
				Logs[i, j] = l;
			}

			var availableTasks = tasks.AllWith("TaskType").Where(e => (e.CompleteDate ?? DateTime.MaxValue) >= Monday).OrderBy(e=>e.TaskType.ShortName).ThenBy(e=>e.Name).ToList();
			AvailableTasks = availableTasks.ToArray();
		}

		DateTime GetMonday(DateTime week)
		{
			while (week.DayOfWeek != DayOfWeek.Monday)
				week -= TimeSpan.FromDays(1);
			return week;
		}

		public static IEnumerable<LogModel> RecentWeeks(int n, Repository<Log> logs)
		{
			for (int i = 0; i < n; ++i)
				yield return new LogModel(DateTime.Today - TimeSpan.FromDays(i * 7), logs);
		}

	}
}