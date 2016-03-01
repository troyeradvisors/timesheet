using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;
namespace Timesheet.Web.Controllers
{
	[AllowAnonymous]
    public class AccountController : Controller
    {
        string HomePage { get { return Url.Action("Index", "Log", null, "http"); } }
        string LoginSite { get { return ConfigurationManager.AppSettings["LoginLink"] + "?ReturnUrl=" + HomePage; } }
        string Hub = ConfigurationManager.AppSettings["ParentHomePage"];
   
		public ActionResult Login()
		{
			return Redirect(LoginSite);
		}

		public ActionResult Logout()
		{
			FormsAuthentication.SignOut();
			return Redirect(LoginSite);
		}

		public ActionResult Home()
		{
			return Redirect(Hub);
		}
    }
}
