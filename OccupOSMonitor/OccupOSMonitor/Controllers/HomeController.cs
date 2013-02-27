using System.Web.Mvc;
using System.Linq;
using NHibernate;


namespace OccupOSMonitor.Controllers {
    using OccupOSMonitor.Infrastructure;
    using OccupOSMonitor.Models;

    public class HomeController : Controller {
        /// <summary>
        /// The _repository.
        /// </summary>[NhSessionManagement]

        public ActionResult Index() {
            return View();
        }
    }
}
