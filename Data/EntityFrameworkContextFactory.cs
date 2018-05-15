using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Starship.Core.DependencyInjection;
using Starship.Core.Interfaces;
using Starship.Core.Utility;

namespace Starship.Azure.Data {
    public class EntityFrameworkContextFactory<T> : IsRepositoryFactory where T : DbContext, new() {

        public EntityFrameworkContextFactory() {
            Id = Guid.NewGuid();
            Types = ReflectionCache.GetTypesOf<HasIdentity>(false).ToList();
        }

        public IEnumerable<Type> GetTypes() {
            return Types;
        }

        public IsRepository GetRepository() {
            var context = DependencyContainer.Get<EntityFrameworkRepository>(Id.ToString());

            if (context == null) {
                context = new EntityFrameworkRepository(new T());
                DependencyContainer.Set(Id.ToString(), context);
            }

            return context;
        }

        private Guid Id { get; set; }

        private IEnumerable<Type> Types { get; set; }
    }
}