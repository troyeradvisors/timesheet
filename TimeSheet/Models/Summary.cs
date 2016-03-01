using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Timesheet.Domain;
using hub = Hub.Domain.Entities ;
using hubRepo = Hub.Domain.Repositories;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Timesheet.Web.Models
{
	public class TaskUserSummary
	{
		public string Name;
		public double Hours;
		public string LastWorkDate;
	}
	public class TaskSummary
	{
		public Task Task;
		public double Hours;
		public List<TaskUserSummary> Users;
	}

	public class Summary
	{
		public Guid UserID;
		public string Task;
		public double TotalHours;
		public double MonthlyHours;
		public double WeeklyHours;
		public double DailyHours;
        public string MissingBusinessDays;

		public static List<Summary> GetSummaries(Guid userID, DateTime begin, DateTime end)
		{
			double totalBusinessDays = CalculateBusinessDays(begin,end);			
			double totalWeeks = totalBusinessDays/5;
			double totalMonths = totalBusinessDays/22.2;
            List<Log> Logs = new Repository<Log>().AllWith("Task").Where(e => e.UserID == userID && e.Date >= begin && e.Date <= end).ToList();
            List<DateTime> missing = new List<DateTime>();
            DateTime cur = begin;
            foreach (var date in Logs.OrderBy(e => e.Date).Select(e=>e.Date).Where(e=>IsBusinessDay(e)).Distinct())
            {
                while (cur.Date <= date.Date)
                {
                    if (cur.Date < date.Date)
                        missing.Add(cur);
                    cur += TimeSpan.FromDays(1);
                    while (!IsBusinessDay(cur)) cur += TimeSpan.FromDays(1);
                }
            }
            while (cur.Date <= end.Date)
            {
                missing.Add(cur);
                cur += TimeSpan.FromDays(1);
                while (!IsBusinessDay(cur)) cur += TimeSpan.FromDays(1);
            }
            string sMissing = string.Join(", ", missing.Select(e => e.ToString("M/d")));
            return Logs.GroupBy(e => e.Task.Name).Select(e => 
				new Summary 
				{
					UserID = userID,
					Task = e.Key, 
					TotalHours = e.Sum(x => x.Hours), 
					MonthlyHours = e.Sum(x => x.Hours) / totalMonths, 
					WeeklyHours = e.Sum(x => x.Hours) / totalWeeks, 
					DailyHours = e.Sum(x => x.Hours) / totalBusinessDays, 
                    MissingBusinessDays = sMissing
				}).OrderBy(e=>e.Task).ToList();
		}

		public static bool IsBusinessDay(DateTime day)
		{
			return day.DayOfWeek != DayOfWeek.Sunday && day.DayOfWeek != DayOfWeek.Saturday;
		}

		public static double CalculateBusinessDays(DateTime begin, DateTime end)
		{
			double calcBusinessDays = 1 + ((end-begin).TotalDays * 5 - (begin.DayOfWeek-end.DayOfWeek) * 2) / 7;
			if ((int)end.DayOfWeek == 6) calcBusinessDays --;
			if ((int)begin.DayOfWeek == 0) calcBusinessDays --;
			return calcBusinessDays;
		}
	}

	public class SummaryModel
	{
		//[DataType(DataType.Date)]
		public DateTime BeginDate { get;set;}

		//[DataType(DataType.Date)]
		public DateTime EndDate { get;set;}

		public List<Summary> Summaries { get; set; }
		public List<TaskSummary> TaskSummaries { get; set; }
	
		public static List<hub.User> Users { get; set; }
		public static List<TaskType> TaskTypes { get; set; }

		public SelectList SelectList { get; set; }
		public SelectList TaskSelectList { get; set; }

		public Guid? ID { get; set; }
		public int? TaskTypeID { get; set; }

		static SummaryModel()
		{
			Users = new hubRepo.Repository<hub.User>().FilterBy(e => e.Client.Name.ToLower() == "troyer advisors").OrderBy(e=>e.FirstName).ToList();
			TaskTypes = new Repository<TaskType>().All.OrderBy(e => e.Name).ToList();
		}

		public SummaryModel()
		{
			if (!HttpContext.Current.User.IsInRole("Admin"))
				ID = hub.User.Current.ID;
			var now = DateTime.Today;
			BeginDate = new DateTime(now.Year, now.Month, 1);
			EndDate = now.AddDays(-1) < BeginDate ? BeginDate : now.AddDays(-1);
		
			SelectList = new SelectList(Users, "ID", "FullName", ID == null ? "" : ID.Value.ToString());
			TaskSelectList = new SelectList(TaskTypes, "ID", "Name", TaskTypeID == null ? "" : TaskTypeID.Value.ToString() );
		}

		public void UpdateSummaries()
		{
			Summaries = Users.SelectMany(e => Summary.GetSummaries(e.ID, BeginDate, EndDate)).ToList();

			var logs = new Repository<Log>().AllWith("Task").ToList();
			
			TaskSummaries =  logs.GroupBy(e => e.Task).Select(e =>
				new TaskSummary
				{
					Task = e.Key,
					Hours = e.Sum(x => x.Hours),
					Users = e.GroupBy(x=> x.UserID).Select(x =>
					{
						var firstOrDefault = Users.FirstOrDefault(u => u.ID==x.Key);
						return new TaskUserSummary() { 
							                                            Hours = x.Sum(xx=>xx.Hours), 
							                                            Name = firstOrDefault == null ? "" : firstOrDefault.LastFirstName,
							                                            LastWorkDate = x.Max(xx=>xx.Date).ToShortDateString()
						                                            };
					}).OrderByDescending(xx=>xx.Hours).ToList()
				}).OrderBy(e => e.Task.CompleteDate).ToList();
		}

	}
}