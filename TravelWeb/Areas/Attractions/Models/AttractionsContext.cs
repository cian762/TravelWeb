using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Attractions.Models;
using TravelWeb.Areas.Activity.Models.EFModel;

namespace TravelWeb.Areas.Attractions.Models;

public partial class AttractionsContext : DbContext
{
    public AttractionsContext()
    {
    }

    public AttractionsContext(DbContextOptions<AttractionsContext> options)
        : base(options)
    {
    }

    // 註冊所有 DbSet
    public virtual DbSet<Attraction> Attractions { get; set; }
    public virtual DbSet<AttractionProduct> AttractionProducts { get; set; }
    public virtual DbSet<AttractionProductDetail> AttractionProductDetails { get; set; }
    public virtual DbSet<AttractionProductFavorite> AttractionProductFavorites { get; set; }
    public virtual DbSet<AttractionTypeCategory> AttractionTypeCategories { get; set; }
    public virtual DbSet<AttractionTypeMapping> AttractionTypeMappings { get; set; }
    public virtual DbSet<Image> Images { get; set; }
    public virtual DbSet<ProductInventoryStatus> ProductInventoryStatuses { get; set; }
    public virtual DbSet<StockInRecord> StockInRecords { get; set; }
    public virtual DbSet<Tag> Tags { get; set; }
    public virtual DbSet<TicketType> TicketTypes { get; set; }
    public virtual DbSet<TagsRegion> TagsRegions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. 景點表設定
        modelBuilder.Entity<Attraction>(entity =>
        {
            entity.ToTable("Attractions", "Attractions");
            entity.HasKey(e => e.AttractionId);

            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.Address).HasMaxLength(255).HasColumnName("address");
            entity.Property(e => e.ApprovalStatus).HasDefaultValue(0).HasColumnName("approval_status");
            entity.Property(e => e.AreaId).HasMaxLength(50).HasColumnName("area_id");
            entity.Property(e => e.BusinessHours).HasMaxLength(500).HasColumnName("business_hours");
            entity.Property(e => e.ClosedDaysNote).HasMaxLength(200).HasColumnName("closed_days_note");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasColumnName("created_at");
            entity.Property(e => e.GooglePlaceId).HasMaxLength(255).HasColumnName("google_place_id");
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 7)").HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10, 7)").HasColumnName("longitude");
            entity.Property(e => e.Name).HasMaxLength(255).HasColumnName("name");
            entity.Property(e => e.OpendataId).HasMaxLength(100).HasColumnName("opendata_id");
            entity.Property(e => e.Phone).HasMaxLength(50).HasColumnName("phone");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.TransportInfo).HasColumnName("transport_info");
            entity.Property(e => e.Website).HasMaxLength(500).HasColumnName("website");

            // 與 TagsRegion 的關聯
            entity.HasOne(d => d.Region)
                .WithMany(p => p.Attractions)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_Attractions_Tags_Regions");
        });

        // 2. 圖片表設定 (核心修復)
        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("Images", "Attractions");
            entity.HasKey(e => e.ImageId);
            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.ImagePath).HasMaxLength(255).HasColumnName("image_path");

            // 景點與圖片的一對多關係
            entity.HasOne(d => d.Attraction)
                .WithMany(p => p.Images)
                .HasForeignKey(d => d.AttractionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Images_Attractions");
        });

        // 3. 區域標籤表設定 (根據截圖修正)
        modelBuilder.Entity<TagsRegion>(entity =>
        {
            entity.ToTable("Tags_Regions", "Activity"); // 修正表名與 Schema
            entity.HasKey(e => e.RegionId);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.Property(e => e.RegionName).HasMaxLength(10);
        });

        // 4. 其他產品相關設定 (保持你原本的邏輯)
        modelBuilder.Entity<AttractionProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.ToTable("AttractionProducts", "Attractions");
            entity.HasIndex(e => e.ProductCode, "UQ_AttractionProducts_ProductCode").IsUnique();
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").HasColumnName("price");
            entity.Property(e => e.ProductCode).HasMaxLength(50).HasColumnName("product_code");

            entity.HasOne(d => d.Attraction).WithMany(p => p.AttractionProducts)
                .HasForeignKey(d => d.AttractionId)
                .HasConstraintName("FK_AttractionProducts_Attractions");
        });

        modelBuilder.Entity<AttractionProductDetail>(entity =>
        {
            entity.HasNoKey().ToTable("AttractionProductDetails", "Attractions");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_AttractionProductDetails_AttractionProducts");
        });

        modelBuilder.Entity<AttractionProductFavorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId);
            entity.ToTable("AttractionProductFavorites", "Attractions");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.HasOne(d => d.Product).WithMany(p => p.AttractionProductFavorites)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_AttractionProductFavorites_AttractionProducts");
        });

        modelBuilder.Entity<AttractionTypeCategory>(entity =>
        {
            entity.HasKey(e => e.AttractionTypeId);
            entity.ToTable("AttractionTypeCategories", "Attractions");
            entity.Property(e => e.AttractionTypeId).HasColumnName("attraction_type_id");
        });

        modelBuilder.Entity<AttractionTypeMapping>(entity =>
        {
            entity.HasNoKey().ToTable("AttractionTypeMappings", "Attractions");
            entity.HasOne(d => d.Attraction).WithMany()
                .HasForeignKey(d => d.AttractionId)
                .HasConstraintName("FK_AttractionTypeMappings_Attractions");
        });

        modelBuilder.Entity<ProductInventoryStatus>(entity =>
        {
            entity.HasNoKey().ToTable("ProductInventoryStatus", "Attractions");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductInventoryStatus_AttractionProducts");
        });

        modelBuilder.Entity<StockInRecord>(entity =>
        {
            entity.HasKey(e => e.StockInId);
            entity.ToTable("StockInRecords", "Attractions");
            entity.Property(e => e.ProductCode).HasMaxLength(50).HasColumnName("product_code");
            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockInRecords)
                .HasPrincipalKey(p => p.ProductCode)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK_StockInRecords_AttractionProducts");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId);
            entity.ToTable("Tags", "Attractions");
            entity.Property(e => e.TagId).HasColumnName("tag_id");
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.TicketTypeCode);
            entity.ToTable("TicketTypes", "Attractions");
            entity.Property(e => e.TicketTypeCode).HasMaxLength(20).HasColumnName("ticket_type_code");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
