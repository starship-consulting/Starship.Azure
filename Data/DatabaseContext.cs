using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace Starship.Azure.Data {
    public class DatabaseContext : DbContext {

        public DatabaseContext() : base("DefaultConnection") {
            Id = Guid.NewGuid();
        }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            var method = modelBuilder.Configurations.GetType().GetMethods().Where(each => each.Name == "Add").ToList()[0];

            var types = new EntityFrameworkContextFactory<DatabaseContext>().GetTypes();

            foreach (var type in types) {
                var configuration = Activator.CreateInstance(typeof (EntityIdentityConfiguration<>).MakeGenericType(type), type.Name);
                method.MakeGenericMethod(type).Invoke(modelBuilder.Configurations, new[] {configuration});
            }

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Conventions.AddBefore<ForeignKeyIndexConvention>(new ForeignKeyNamingConvention());

            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
            
            base.OnModelCreating(modelBuilder);
        }
        
        public Guid Id { get; set; }
    }
}