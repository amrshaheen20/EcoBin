using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace EcoBin.API.Models.DbSet
{
    public class WorkerTask : BaseEntity
    {
        public required int WorkerId { get; set; }
        public required int CreatedById { get; set; }
        public required int TranchBinId { get; set; }
        [StringLength(255)]
        public required string Title { get; set; }
        [StringLength(500)]
        public required string Description { get; set; }
        public required DateTime DueDate { get; set; }
        public required bool IsCompleted { get; set; } = false;

        public virtual Worker Worker { get; set; } = default!;
        public virtual User CreatedBy { get; set; } = default!;
        public virtual TranchBin TranchBin { get; set; } = default!;


    }

    public class WorkerTaskConfiguration : IEntityTypeConfiguration<WorkerTask>
    {
        public void Configure(EntityTypeBuilder<WorkerTask> builder)
        {
            builder.HasIndex(x => x.WorkerId);
            builder.HasIndex(x => x.CreatedById);

            builder.HasOne(x => x.Worker)
                   .WithMany()
                   .HasForeignKey(x => x.WorkerId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.CreatedBy)
                   .WithMany()
                   .HasForeignKey(x => x.CreatedById)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
