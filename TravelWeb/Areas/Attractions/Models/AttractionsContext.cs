using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Attractions.Models;
using TravelWeb.Models;

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
    public virtual DbSet<AttractionProductTag> AttractionProductTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                // 關鍵判斷：只有當「還沒有手動設定過欄位名稱」時，才進行自動轉換
                if (string.IsNullOrEmpty(property.GetColumnName(StoreObjectIdentifier.Table(entity.GetTableName(), entity.GetSchema()))))
                {
                    var columnName = string.Concat(property.Name.Select((x, i) =>
                        i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
                    property.SetColumnName(columnName);
                }
            }
        }
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
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            // 與 TagsRegion 的關聯
            entity.HasOne(d => d.Region)
                .WithMany(p => p.Attractions)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_Attractions_Tags_Regions");

            // 2. 景點票券產品表設定
            modelBuilder.Entity<AttractionProduct>(entity =>
            {
                entity.ToTable("AttractionProducts", "Attractions"); // 指定 Schema 為 Attractions
                entity.HasKey(e => e.ProductId);

                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.ProductCode).HasColumnName("product_code").HasMaxLength(50);
                entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
               
                entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255);
                entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(50);
                entity.Property(e => e.PolicyId).HasColumnName("policy_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("datetime2(7)");
                entity.Property(e => e.Price).HasColumnName("price").HasColumnType("decimal(10, 2)");
                entity.Property(e => e.MaxPurchaseQuantity).HasColumnName("max_purchase_quantity");
                entity.Property(e => e.IsActive).HasColumnName("is_active");
                entity.Property(e => e.TicketTypeCode).HasColumnName("ticket_type_code");

                // 設定與 Attraction 的關聯
                entity.HasOne(d => d.Attraction)
                    .WithMany(p => p.AttractionProducts) // 確保 Attraction Model 裡有這個 ICollection
                    .HasForeignKey(d => d.AttractionId)
                    .HasConstraintName("FK_AttractionProducts_Attractions");
            });


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
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            // 關鍵修正：明確指定外鍵對應
            entity.HasOne(d => d.TicketType)
                  .WithMany(p => p.AttractionProducts)
                  .HasForeignKey(d => d.TicketTypeCode) // 告訴 EF 你的外鍵是這一欄
                  .HasConstraintName("FK_AttractionProducts_TicketTypes");
        });

        modelBuilder.Entity<AttractionProductDetail>(entity =>
        {
            // 1. 設定 product_id 為主鍵 (雖然 DB 沒設，但 EF 需要它來追蹤)
            entity.HasKey(e => e.ProductId);

            entity.ToTable("AttractionProductDetails", "Attractions");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ContentDetails).HasColumnName("content_details");
            entity.Property(e => e.UsageInstructions).HasColumnName("usage_instructions");
            entity.Property(e => e.LastUpdatedAt).HasColumnName("last_updated_at");

            // 2. 正確設定 1 對 1 關係
            entity.HasOne(d => d.Product)
                  .WithOne(p => p.AttractionProductDetail) // 指向剛才在 Product 補上的屬性
                  .HasForeignKey<AttractionProductDetail>(d => d.ProductId)
                  .HasConstraintName("FK_AttractionProductDetails_AttractionProducts");
        });

        modelBuilder.Entity<AttractionProductFavorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId);
            entity.ToTable("AttractionProductFavorites", "Attractions");

            // ↓ 補上這兩個缺少的欄位對應
            entity.Property(e => e.FavoriteId).HasColumnName("favorite_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.Property(e => e.UserId).HasColumnName("user_id").HasMaxLength(50);
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            // DB 有設唯一約束，建議加上避免重複收藏
            entity.HasIndex(e => new { e.UserId, e.ProductId })
                  .IsUnique()
                  .HasDatabaseName("UQ_Favorites_UserProduct");

            entity.HasOne(d => d.Product)
                  .WithMany(p => p.AttractionProductFavorites)
                  .HasForeignKey(d => d.ProductId)
                  .OnDelete(DeleteBehavior.Cascade)  // ↓ 建議補上，對應 DB 設定
                  .HasConstraintName("FK_AttractionProductFavorites_AttractionProducts");

            // 方案一：不加 Member 的 HasOne 關係，只靠 UserId 字串對應
        });

        modelBuilder.Entity<AttractionTypeCategory>(entity =>
        {
            entity.HasKey(e => e.AttractionTypeId);
            entity.ToTable("AttractionTypeCategories", "Attractions");
            entity.Property(e => e.AttractionTypeId).HasColumnName("attraction_type_id");
            entity.Property(e => e.AttractionTypeName).HasMaxLength(100).HasColumnName("attraction_type_name");
        });

        modelBuilder.Entity<AttractionTypeMapping>(entity =>
        {
            // 移除 HasNoKey()，改用 HasKey 複合鍵
            entity.ToTable("AttractionTypeMappings", "Attractions");
            entity.HasKey(e => new { e.AttractionId, e.AttractionTypeId });

            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.AttractionTypeId).HasColumnName("attraction_type_id");

            entity.HasOne(d => d.Attraction)
                .WithMany(p => p.AttractionTypeMappings)
                .HasForeignKey(d => d.AttractionId);
        });

        modelBuilder.Entity<ProductInventoryStatus>(entity =>
        {
            entity.HasKey(e => e.ProductId);  // ← 從 HasNoKey() 改成這個
            entity.ToTable("ProductInventoryStatus", "Attractions");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.InventoryMode).HasColumnName("inventory_mode").HasMaxLength(20);
            entity.Property(e => e.DailyLimit).HasColumnName("daily_limit");
            entity.Property(e => e.SoldQuantity).HasColumnName("sold_quantity");
            entity.Property(e => e.LastUpdatedAt).HasColumnName("last_updated_at");

            entity.HasOne(d => d.Product)
                  .WithMany()
                  .HasForeignKey(d => d.ProductId)
                  .HasConstraintName("FK_ProductInventoryStatus_AttractionProducts");
        });

        modelBuilder.Entity<StockInRecord>(entity =>
        {
            entity.HasKey(e => e.StockInId);
            entity.ToTable("StockInRecords", "Attractions");

            entity.Property(e => e.StockInId).HasColumnName("stock_in_id");
            entity.Property(e => e.ProductType).HasColumnName("product_type").HasMaxLength(20);
            entity.Property(e => e.ProductCode).HasColumnName("product_code").HasMaxLength(50);
            entity.Property(e => e.SupplierName).HasColumnName("supplier_name").HasMaxLength(100);
            entity.Property(e => e.UnitCost).HasColumnName("unit_cost").HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RemainingStock).HasColumnName("remaining_stock");
            entity.Property(e => e.InventoryType).HasColumnName("inventory_type").HasMaxLength(20);
            entity.Property(e => e.StockInDate).HasColumnName("stock_in_date");
            entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);

            entity.HasOne(d => d.ProductCodeNavigation)
                  .WithMany(p => p.StockInRecords)
                  .HasPrincipalKey(p => p.ProductCode)
                  .HasForeignKey(d => d.ProductCode)
                  .HasConstraintName("FK_StockInRecords_AttractionProducts");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId);

            // 根據你的圖表，如果是在 Attractions 結構圖中，請確認 Schema 名稱
            entity.ToTable("Tags", "Attractions");

            entity.Property(e => e.TagId).HasColumnName("tag_id");

            // 補上標籤名稱的對應
            entity.Property(e => e.TagName)
                  .HasMaxLength(50) // 假設長度為 50
                  .HasColumnName("tag_name");
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.TicketTypeCode);
            entity.ToTable("TicketTypes", "Attractions");

            // 移除 HasMaxLength，因為 int 不支援長度設定
            entity.Property(e => e.TicketTypeCode)
                  .HasColumnName("ticket_type_code");

            entity.Property(e => e.TicketTypeName)
                  .HasMaxLength(50) // 對應你的 nvarchar(50)
                  .HasColumnName("ticket_type_name");

            // 關鍵！加上這行來對應資料庫欄位
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
        });

        modelBuilder.Entity<AttractionProductTag>(entity =>
        {
            // 1. 設定複合主鍵 (PK) - 對應 SQL 中的兩欄組合 PK
            entity.HasKey(e => new { e.ProductId, e.TagId });

            // 2. 指定正確的 Schema (Attractions) 與 表名
            entity.ToTable("AttractionProductTags", "Attractions");

            // 3. 鎖定資料庫欄位名稱，確保與 SQL 裡的底線格式完全一致
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");

            // 4. 設定與 Product 的一對多關係
            entity.HasOne(d => d.Product)
                .WithMany(p => p.AttractionProductTags)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade) // 刪除產品時同步移除標籤關聯
                .HasConstraintName("FK_AttractionProductTags_AttractionProducts");

            // 5. 設定與 Tag 的一對多關係
            entity.HasOne(d => d.Tag)
                .WithMany(p => p.AttractionProductTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AttractionProductTags_Tags");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
