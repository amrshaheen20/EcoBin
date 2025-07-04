﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace EcoBin.API.Models.DbSet
{
    public class UserSession : BaseEntity
    {
        public int UserId { get; set; }
        [StringLength(250)]
        [DefaultValueSql("('')")]
        public string TokenId { get; set; } = default!;
        public DateTime ExpirationTime { get; set; }
        [StringLength(500)]
        [DefaultValueSql("('')")]
        public string? UserAgent { get; set; }
        public virtual User User { get; set; } = default!;
    }

    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {

            builder.HasIndex(x => new { x.UserId, x.TokenId, x.ExpirationTime })
                .IsUnique();

            builder.HasIndex(x => x.UserId);
            builder.HasIndex(x => x.TokenId);
        }
    }
}

