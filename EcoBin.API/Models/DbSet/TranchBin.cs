using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace EcoBin.API.Models.DbSet
{
    public class TranchBin : BaseEntity
    {
        [StringLength(255)]
        public required string Name { get; set; }
        [StringLength(500)]
        public required string Location { get; set; }
        public required int MaxCapacity { get; set; }
        public int CurrentCapacity { get; set; } = 0;
        public int CreatedById { get; set; }
        public bool IsLidOpen { get; set; } = false;
        public bool IsMaintenanceMode { get; set; } = false;
        public string? Token { get; set; } = null;
        public virtual User CreatedBy { get; set; } = default!;
        public virtual ICollection<WorkerTask> Tasks { get; set; } = new List<WorkerTask>();
    }

    public class TranchBinConfiguration : IEntityTypeConfiguration<TranchBin>
    {
        public void Configure(EntityTypeBuilder<TranchBin> builder)
        {
            builder.HasIndex(x => x.Name).IsUnique();
            builder.HasIndex(x => x.Location);
            builder.HasOne(x => x.CreatedBy)
                   .WithMany()
                   .HasForeignKey(x => x.CreatedById)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Tasks)
                     .WithOne(x => x.TranchBin)
                     .HasForeignKey(x => x.TranchBinId)
                     .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
