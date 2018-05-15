using System;
using System.Linq;

namespace Starship.Azure.Data {
    
    public interface IsRepository : IDisposable {
        
        Guid Id { get; set; }

        void Commit();

        void Delete<T>(T entity) where T : class;

        IQueryable Query(Type type);
        
        T Add<T>(T entity) where T : class;
        
        object Find(Type type, object id);
    }

    public static class IsRepositoryExtensions {
        public static IQueryable<T> Query<T>(this IsRepository context) where T : class {
            return context.Query(typeof (T)) as IQueryable<T>;
        }

        public static T Find<T>(this IsRepository context, object id) where T : class {
            return context.Find(typeof(T), id) as T;
        }
    }
}