using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Starship.Azure.Providers.Tables {
    public interface ITableStorage {
        Task Save(string tableName, object entity);

        Task<List<T>> All<T>(string tableName);

        Task<List<T>> Query<T>(string tableName, Func<T, bool> predicate) where T : IPartitionedEntity;
    }
}