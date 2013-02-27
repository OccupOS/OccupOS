using System.Linq;

namespace OccupOSMonitor.Models {
    public interface IRepository<TEntity> where TEntity : Entity {
        TEntity Get(int id);
        int Add(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
        IQueryable<TEntity> Items { get; }
    }
}