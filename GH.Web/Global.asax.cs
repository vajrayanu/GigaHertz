using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using GH.DAL.Context;
using System.Globalization;



namespace GH.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            //Custom route for reports
            routes.MapPageRoute(
                 "Reports",                         // Route name
                 "Reports/{reportname}",                // URL
                 "~/Reports/{reportname}.aspx"   // File
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            Database.SetInitializer<DataContext>(null);
            Database.SetInitializer<DataContext2>(null);
           

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            //System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("th-TH");
        }
    }
}