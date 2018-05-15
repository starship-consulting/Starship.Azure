using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Starship.Azure.Data {
    public class EntityFrameworkRepository : IsRepository {
        
        public EntityFrameworkRepository(DbContext context) {
            Id = Guid.NewGuid();
            Context = context;
        }

        public Guid Id { get; set; }

        public void Dispose() {
        }

        public void Commit() {
            Context.SaveChanges();
        }

        public void Delete<T>(T entity) where T : class {
            Context.Set(entity.GetType()).Remove(entity);
        }

        public IQueryable Query(Type type) {
            return Context.Set(type);
        }

        public T Add<T>(T entity) where T : class {
            Context.Set<T>().Add(entity);
            return entity;
        }
        
        public object Find(Type type, object id) {
            return Context.Set(type).Find(id);
        }
        
        public TProperty Load<T, TProperty>(T entity, Expression<Func<T, TProperty>> property) where T : class where TProperty : class {
            throw new NotImplementedException();
        }
        
        public IEnumerable<Type> GetTypes() {
            throw new NotImplementedException();
        }

        public IsRepository GetContext() {
            return this;
        }

        private DbContext Context { get; set; }
    }
}