using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace EcoBin.API.Models.DbSet
{
    public class Report : BaseEntity
    {
        public required int MangerId { get; set; }
        public required int WorkerId { get; set; }
        [StringLength(255)]
        public required string Title { get; set; }
        [StringLength(500)]
        public required string Message { get; set; }
        public virtual User Manger { get; set; } = default!;
        public virtual Worker Worker { get; set; } = default!;
    }
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.HasIndex(x => x.MangerId);
            builder.HasOne(x => x.Manger)
                   .WithMany(x => x.Reports)
                   .HasForeignKey(x => x.MangerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Worker)
                     .WithMany(x => x.Reports)
                     .HasForeignKey(x => x.WorkerId)
                     .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
