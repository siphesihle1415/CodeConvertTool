using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CodeConverterTool.Models;

public partial class ConvertToolDbContext : DbContext
{
    public ConvertToolDbContext()
    {
    }

    public ConvertToolDbContext(DbContextOptions<ConvertToolDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<Script> Scripts { get; set; }

    public virtual DbSet<Scripttypelookup> Scripttypelookups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.DevId).HasName("PK__develope__8AB90B4949C53C0F");

            entity.ToTable("developers");

            entity.Property(e => e.DevId).HasColumnName("dev_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Script>(entity =>
        {
            entity.HasKey(e => e.ScriptId).HasName("PK__scripts__EDFCC9DF1DA27B3B");

            entity.ToTable("scripts");

            entity.Property(e => e.ScriptId).HasColumnName("script_id");
            entity.Property(e => e.DevId).HasColumnName("dev_id");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_updated");
            entity.Property(e => e.ScriptName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("script_name");
            entity.Property(e => e.ScriptS3Uri)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("script_s3_uri");
            entity.Property(e => e.ScriptType).HasColumnName("script_type");
            entity.Property(e => e.ScriptVersion)
                .HasDefaultValue(1)
                .HasColumnName("script_version");

            entity.HasOne(d => d.Dev).WithMany(p => p.Scripts)
                .HasForeignKey(d => d.DevId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__scripts__dev_id__276EDEB3");

            entity.HasOne(d => d.ScriptTypeNavigation).WithMany(p => p.Scripts)
                .HasForeignKey(d => d.ScriptType)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__scripts__script___286302EC");
        });

        modelBuilder.Entity<Scripttypelookup>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__scriptty__2C0005986CC1562B");

            entity.ToTable("scripttypelookup");

            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("type");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
