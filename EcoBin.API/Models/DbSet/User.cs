using EcoBin.API.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
namespace EcoBin.API.Models.DbSet
{
    public class User : BaseEntity
    {
        //data for login
        [StringLength(255)]
        public required string Name { get; set; }
        [StringLength(255)]
        public required string Email { get; set; }
        public bool IsEmailConfirmed { get; set; } = true;
        public required eRole Role { get; set; }

        [StringLength(500)]
        public required string PasswordHash { get; set; }


        [StringLength(255)]
        public string? PhoneNumber { get; set; }

        [StringLength(255)]
        public string? CompanyName { get; set; }

        [DefaultValueSql("getutcdate()")]
        public DateTime LastActiveTime { get; set; }

        public virtual ICollection<UserSession> Sessions { get; set; } = new List<UserSession>();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
        public virtual ICollection<Worker> Workers { get; set; } = new List<Worker>();

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Role);

            builder.Property(x=>x.Role).HasConversion(
                       v => v.ToString(),
                       v => (eRole)Enum.Parse(typeof(eRole), v))
                   .IsRequired();

            builder.HasMany(x => x.Sessions)
                     .WithOne(x => x.User)
                     .HasForeignKey(x => x.UserId)
                     .OnDelete(DeleteBehavior.Cascade);
        }
    }




}
