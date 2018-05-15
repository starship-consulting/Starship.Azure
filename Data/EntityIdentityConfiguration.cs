using System.Data.Entity.ModelConfiguration;
using Starship.Core.Interfaces;

namespace Starship.Azure.Data {
    public class EntityIdentityConfiguration<T> : EntityTypeConfiguration<T> where T : class, HasIdentity {
        public EntityIdentityConfiguration(string tableName) {
            Map(a => a.MapInheritedProperties());
            HasKey(p => p.Id);
            ToTable(tableName);
        }
    }
}