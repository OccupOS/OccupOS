using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OccupOSMonitor.Models;
using System.Web.Http.Dependencies;

namespace OccupOSMonitor.Infrastructure {


    public class SimpleRepositoryResolver<T> : IDependencyResolver
        where T : Entity {
        public void Dispose() {
        }

        public object GetService(Type serviceType) {
            if (serviceType == typeof(IRepository<T>)) {
                return new NHibernateRepository<T>() {
                };
            }
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType) {
            return new List<object>();
        }

        public IDependencyScope BeginScope() {
            return this;
        }
    }
}