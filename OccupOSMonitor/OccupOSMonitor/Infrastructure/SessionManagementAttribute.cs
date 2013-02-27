using System;
using System.Web.Http.Controllers;

namespace OccupOSMonitor.Infrastructure {
    public class SessionManagementAttribute : Attribute, IControllerConfiguration {
        public void Initialize(HttpControllerSettings controllerSettings,
                               HttpControllerDescriptor controllerDescriptor) {
            controllerDescriptor.Configuration.Filters.Add(new NhSessionManagementAttribute());
            //controllerDescriptor.Configuration.DependencyResolver
            //controllerSettings.Services.
        }
    }
}