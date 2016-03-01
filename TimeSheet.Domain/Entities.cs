using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.IO;
using System.Web;

namespace Timesheet.Domain
{
	[MetadataType(typeof(Log.Metadata))]
	public partial class Log
	{
		class Metadata
		{
		}
	}

	[MetadataType(typeof(Task.Metadata))]
	public partial class Task
	{
		class Metadata
		{
			[Required]
			public string Name { get; set; }
			public DateTime CreateDate { get; set; }
			public DateTime CompleteDate { get; set; }
		}

		public Task() { }
		public string ListDisplay { get { return this.TaskType.ShortName + " - " + Name; } }
	}

	[MetadataType(typeof(TaskType.Metadata))]
	public partial class TaskType
	{
		class Metadata
		{
			[Required]
			public string Name { get; set; }
			[Required]
			[DisplayName("Abbr")]
			public string ShortName { get; set; }
		}
	}

}