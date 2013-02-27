// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="UCL">
//   Open Source
// </copyright>
// <summary>
//   Defines the WebApiApplication type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using System.Data.SqlClient;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;

using OccupOSMonitor.App_Start;
using OccupOSMonitor.Infrastructure;
using OccupOSMonitor.Controllers;

namespace OccupOSMonitor {

    /// <summary>
    /// The web api application.
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication {

        /// <summary>
        /// The application_ start.
        /// </summary>
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = true;

            //Per action session. See: http://www.piotrwalat.net/nhibernate-session-management-in-asp-net-web-api/
            //InitializeSessionFactory();
            //GlobalConfiguration.Configuration.Filters.Add(new NhSessionManagementAttribute());
            //log4net.Config.XmlConfigurator.Configure();
            //SqlDependency.Start(ConfigurationManager.ConnectionStrings["default"].ConnectionString);
        }

        //protected void Application_End(object sender, EventArgs e) {
        //    SqlDependency.Stop(ConfigurationManager.ConnectionStrings["default"].ConnectionString);
        //}
    }
}