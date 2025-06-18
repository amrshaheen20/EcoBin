using EcoBin.API.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace EcoBin.API.Models.DbSet
{
    public class Notification : BaseEntity
    {
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        [StringLength(255)]
        public eNotificationType Type { get; set; } = eNotificationType.Info;

        public int RecipientUserId { get; set; }

        public virtual User RecipientUser { get; set; } = null!;
    }

    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasIndex(x => x.RecipientUserId);

            builder.HasOne(x => x.RecipientUser)
                   .WithMany(x => x.Notifications)
                   .HasForeignKey(x => x.RecipientUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Type)
                     .HasConversion(
                          v => v.ToString(),
                          v => (eNotificationType)Enum.Parse(typeof(eNotificationType), v))
                     .IsRequired();

        }
    }
}
