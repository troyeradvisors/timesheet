using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RepoInfrastructure.Concrete;
using System.Data.Objects.DataClasses;

namespace Timesheet.Domain
{
	public class Repository<T> : ContextRepository<T, TimesheetEntities> where T : EntityObject { }

}
