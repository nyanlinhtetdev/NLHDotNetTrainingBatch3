using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace NLHDotNetTrainingBatch3.ProductnSaleDatabase.AppDbContextModels;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<TblProduct> TblProducts { get; set; }

    public virtual DbSet<TblSale> TblSales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ProductC__19093A2B4F422F3E");

            entity.ToTable("ProductCategory");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<TblProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.ToTable("Tbl_Product");

            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
            entity.Property(e => e.ModifiedDateTime).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.TblProducts)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Product_ProductCategory");
        });

        modelBuilder.Entity<TblSale>(entity =>
        {
            entity.HasKey(e => e.SaleId);

            entity.ToTable("Tbl_Sale");

            entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 0)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
