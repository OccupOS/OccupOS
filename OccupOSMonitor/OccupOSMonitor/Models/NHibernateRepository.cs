using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NHibernate;
using NHibernate.Linq;
using OccupOSMonitor.Infrastructure;

namespace OccupOSMonitor.Models {

    //[NhSessionManagement]
    public class NHibernateRepository<TEntity> : IRepository<TEntity> where TEntity : Entity {
        private readonly ISession _session;

        public NHibernateRepository() {
            // you may want to use dependency injection instead
            //_session = WebApiApplication.SessionFactory.GetCurrentSession();
        }

        public TEntity Get(int id) {
            return _session.Get<TEntity>(id);
        }

        public int Add(TEntity entity) {
            _session.Save(entity);
            return entity.Id;
        }

        public void Delete(TEntity entity) {
            _session.Delete(entity);
        }

        public void Update(TEntity entity) {
            _session.Update(entity);
        }

        public IQueryable<TEntity> Items {
            get { return _session.Query<TEntity>(); }
        }
    }
}