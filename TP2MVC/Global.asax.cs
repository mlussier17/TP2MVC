using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TP2MVC.Models;

namespace TP2MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            string DB_Path = Server.MapPath(@"~\App_Data\Users.mdf");
            String MainDB = @"Data Source=(LocalDB)\v11.0;AttachDbFilename='" + DB_Path + "'; Integrated Security=true;Max Pool Size=1024;Pooling=true;";

            HttpRuntime.Cache["Users"] = new Users(MainDB);
        }
        protected void Session_End()
        {
            try
            {
                HttpRuntime.Cache["OnLineUsersLastUpdate"] = DateTime.Now;
            }
            catch (Exception)
            {
            }
            Session.Clear();
        }
    }
}
