using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using EcoBin.API.Models.DbSet;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace EcoBin.API
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DefaultValueSqlAttribute : Attribute
    {
        public string Sql { get; }
        public DefaultValueSqlAttribute(string sql) => Sql = sql;
    }
    public class DataBaseContext(DbContextOptions<DataBaseContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<WorkerTask> Tasks { get; set; }
        public DbSet<TranchBin> TranchBins { get; set; }








        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            ApplyDefaultValueSqlAttributes(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }


        private void ApplyDefaultValueSqlAttributes(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (clrType == null)
                    continue;

                var entityBuilder = modelBuilder.Entity(clrType);

                var properties = clrType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var prop in properties)
                {
                    var attr = prop.GetCustomAttribute<DefaultValueSqlAttribute>(inherit: true);
                    if (attr != null)
                    {
                        entityBuilder.Property(prop.Name).HasDefaultValueSql(attr.Sql);
                    }
                }
            }
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

    }
}
