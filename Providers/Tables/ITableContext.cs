using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Starship.Azure.Providers.Tables {
    public interface ITableContext<T> where T : new() {
        Task<T> SaveAsync(T entity);
        Task<List<ITableResult>> Save(List<T> entities, bool insertOrUpdate);
        IQueryable<T> Query();
        Task<IEnumerable<T>> QueryAsync(IQueryable<T> query);
    }
}