using EcoBin.API.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EcoBin.API.Models.DbSet
{
    public class Worker : BaseEntity
    { 
        public required int UserId { get; set; }
        public required int CreatedById { get; set; }
        public required eWorkerJobType jobType { get; set; }
        public virtual User User { get; set; } = default!;
        public virtual User CreatedBy { get; set; } = default!;

        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    }

    public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
    {
        public void Configure(EntityTypeBuilder<Worker> builder)
        {
            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.CreatedById);
            builder.HasIndex(x => x.jobType);

            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.CreatedBy)
                   .WithMany(x => x.Workers)
                   .HasForeignKey(x => x.CreatedById)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
