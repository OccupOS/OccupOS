using Funq;
using ServiceStack.OrmLite;
using ServiceStack.WebHost.Endpoints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;


namespace OccupOSMonitorNew
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class TestHostApp : AppHostBase
    {
        public TestHostApp() : base("Hello Web Services", typeof(SensorDataService).Assembly) { }

        public override void Configure(Container container)
        {
     /*       container.Register<IDbConnectionFactory>(c =>
                new OrmLiteConnectionFactory(
    "Data Source=tcp:dndo40zalb.database.windows.net,1433;Initial Catalog=TestSQLDB;User ID=comp2014@dndo40zalb;Password=20041908kjH;Trusted_Connection=False;Encrypt=True;Connection Timeout=30;",  //Connection String
    SqlServerDialect.Provider));*/

            //register any dependencies your services use, e.g:
            //container.Register<ICacheClient>(new MemoryCacheClient());
        }
    }
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            new TestHostApp().Init();
        }
    }
}