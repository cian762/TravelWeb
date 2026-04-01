using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Attractions.Models;
using TravelWeb.Models;

namespace TravelWeb.Areas.Attractions.Models;

public partial class AttractionsContext : DbContext
{
    public AttractionsContext() { }

    public AttractionsContext(DbContextOptions<AttractionsContext> options)
        : base(options) { }

    public virtual DbSet<Attraction> Attractions { get; set; }
    public virtual DbSet<AttractionProduct> AttractionProducts { get; set; }
    public virtual DbSet<AttractionProductDetail> AttractionProductDetails { get; set; }
    public virtual DbSet<AttractionProductFavorite> AttractionProductFavorites { get; set; }
    public virtual DbSet<AttractionProductImage> AttractionProductImages { get; set; }  // ← 新增
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
        // ── 1. 景點表 ─────────────────────────────────────────────
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
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ActivityIntro).HasColumnName("activity_intro");  // ← 新增
            entity.Property(e => e.Website).HasMaxLength(500).HasColumnName("website");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");

            entity.HasOne(d => d.Region)
                .WithMany(p => p.Attractions)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_Attractions_Tags_Regions");
        });

        // ── 2. 票券產品表 ────────────────────────────────────────
        modelBuilder.Entity<AttractionProduct>(entity =>
        {
            entity.ToTable("AttractionProducts", "Attractions");
            entity.HasKey(e => e.ProductId);
            entity.HasIndex(e => e.ProductCode, "UQ_AttractionProducts_ProductCode").IsUnique();

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductCode).HasMaxLength(50).HasColumnName("product_code");
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.Title).HasMaxLength(255).HasColumnName("title");
            entity.Property(e => e.Status).HasMaxLength(50).HasColumnName("status");
            entity.Property(e => e.PolicyId).HasColumnName("policy_id");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime2(7)").HasColumnName("created_at");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)").HasColumnName("price");
            entity.Property(e => e.OriginalPrice).HasColumnType("decimal(10, 2)").HasColumnName("original_price");
            entity.Property(e => e.ValidityDays).HasColumnName("validity_days");
            entity.Property(e => e.MaxPurchaseQuantity).HasColumnName("max_purchase_quantity");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.TicketTypeCode).HasColumnName("ticket_type_code");

            entity.HasOne(d => d.Attraction)
                .WithMany(p => p.AttractionProducts)
                .HasForeignKey(d => d.AttractionId)
                .HasConstraintName("FK_AttractionProducts_Attractions");

            entity.HasOne(d => d.TicketType)
                .WithMany(p => p.AttractionProducts)
                .HasForeignKey(d => d.TicketTypeCode)
                .HasConstraintName("FK_AttractionProducts_TicketTypes");
        });

        // ── 3. 票券詳情表 ────────────────────────────────────────
        modelBuilder.Entity<AttractionProductDetail>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.ToTable("AttractionProductDetails", "Attractions");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ContentDetails).HasColumnName("content_details");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.UsageInstructions).HasColumnName("usage_instructions");
            entity.Property(e => e.Includes).HasColumnName("includes");
            entity.Property(e => e.Excludes).HasColumnName("excludes");
            entity.Property(e => e.Eligibility).HasColumnName("eligibility");
            entity.Property(e => e.CancelPolicy).HasMaxLength(500).HasColumnName("cancel_policy");
            entity.Property(e => e.ValidityNote).HasMaxLength(500).HasColumnName("validity_note");
            entity.Property(e => e.LastUpdatedAt).HasColumnName("last_updated_at");

            entity.HasOne(d => d.Product)
                .WithOne(p => p.AttractionProductDetail)
                .HasForeignKey<AttractionProductDetail>(d => d.ProductId)
                .HasConstraintName("FK_AttractionProductDetails_AttractionProducts");
        });

        // ── 4. 圖片表 ────────────────────────────────────────────
        modelBuilder.Entity<Image>(entity =>
        {
            entity.ToTable("Images", "Attractions");
            entity.HasKey(e => e.ImageId);
            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.ImagePath).HasMaxLength(255).HasColumnName("image_path");

            entity.HasOne(d => d.Attraction)
                .WithMany(p => p.Images)
                .HasForeignKey(d => d.AttractionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Images_Attractions");
        });

        // ── 4b. 票券活動介紹圖片表 ───────────────────────────────  ← 新增
        modelBuilder.Entity<AttractionProductImage>(entity =>
        {
            entity.ToTable("AttractionProductImages", "Attractions");
            entity.HasKey(e => e.ImageId);

            entity.Property(e => e.ImageId).HasColumnName("image_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ImagePath).HasMaxLength(500).HasColumnName("image_path");
            entity.Property(e => e.Caption).HasMaxLength(500).HasColumnName("caption");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.AttractionProductImages)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AttractionProductImages_AttractionProducts");
        });

        // ── 5. 區域標籤表 ────────────────────────────────────────
        modelBuilder.Entity<TagsRegion>(entity =>
        {
            entity.ToTable("Tags_Regions", "Activity");
            entity.HasKey(e => e.RegionId);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Uid).HasColumnName("UID");
            entity.Property(e => e.RegionName).HasMaxLength(10);
        });

        // ── 6. 收藏表 ────────────────────────────────────────────
        modelBuilder.Entity<AttractionProductFavorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId);
            entity.ToTable("AttractionProductFavorites", "Attractions");
            entity.Property(e => e.FavoriteId).HasColumnName("favorite_id");
            entity.Property(e => e.UserId).HasMaxLength(50).HasColumnName("user_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.HasIndex(e => new { e.UserId, e.ProductId })
                .IsUnique().HasDatabaseName("UQ_Favorites_UserProduct");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.AttractionProductFavorites)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AttractionProductFavorites_AttractionProducts");
        });

        // ── 7. 景點分類表 ────────────────────────────────────────
        modelBuilder.Entity<AttractionTypeCategory>(entity =>
        {
            entity.HasKey(e => e.AttractionTypeId);
            entity.ToTable("AttractionTypeCategories", "Attractions");
            entity.Property(e => e.AttractionTypeId).HasColumnName("attraction_type_id");
            entity.Property(e => e.AttractionTypeName).HasMaxLength(100).HasColumnName("attraction_type_name");
        });

        // ── 8. 景點分類對應表 ─────────────────────────────────────
        modelBuilder.Entity<AttractionTypeMapping>(entity =>
        {
            entity.ToTable("AttractionTypeMappings", "Attractions");
            entity.HasKey(e => new { e.AttractionId, e.AttractionTypeId });
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.AttractionTypeId).HasColumnName("attraction_type_id");

            entity.HasOne(d => d.Attraction)
                .WithMany(p => p.AttractionTypeMappings)
                .HasForeignKey(d => d.AttractionId);
        });

        // ── 9. 庫存狀態表 ────────────────────────────────────────
        modelBuilder.Entity<ProductInventoryStatus>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.ToTable("ProductInventoryStatus", "Attractions");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.InventoryMode).HasMaxLength(20).HasColumnName("inventory_mode");
            entity.Property(e => e.DailyLimit).HasColumnName("daily_limit");
            entity.Property(e => e.SoldQuantity).HasColumnName("sold_quantity");
            entity.Property(e => e.LastUpdatedAt).HasColumnName("last_updated_at");

            entity.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductInventoryStatus_AttractionProducts");
        });

        // ── 10. 進貨紀錄表 ───────────────────────────────────────
        modelBuilder.Entity<StockInRecord>(entity =>
        {
            entity.HasKey(e => e.StockInId);
            entity.ToTable("StockInRecords", "Attractions");
            entity.Property(e => e.StockInId).HasColumnName("stock_in_id");
            entity.Property(e => e.ProductType).HasMaxLength(20).HasColumnName("product_type");
            entity.Property(e => e.ProductCode).HasMaxLength(50).HasColumnName("product_code");
            entity.Property(e => e.SupplierName).HasMaxLength(100).HasColumnName("supplier_name");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(10, 2)").HasColumnName("unit_cost");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RemainingStock).HasColumnName("remaining_stock");
            entity.Property(e => e.InventoryType).HasMaxLength(20).HasColumnName("inventory_type");
            entity.Property(e => e.StockInDate).HasColumnName("stock_in_date");
            entity.Property(e => e.Remarks).HasMaxLength(500).HasColumnName("remarks");

            entity.HasOne(d => d.ProductCodeNavigation)
                .WithMany(p => p.StockInRecords)
                .HasPrincipalKey(p => p.ProductCode)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK_StockInRecords_AttractionProducts");
        });

        // ── 11. 標籤表 ───────────────────────────────────────────
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId);
            entity.ToTable("Tags", "Attractions");
            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.TagName).HasMaxLength(50).HasColumnName("tag_name");
            entity.Property(e => e.Description).HasMaxLength(500).HasColumnName("description");
        });

        // ── 12. 票種表 ───────────────────────────────────────────
        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.TicketTypeCode);
            entity.ToTable("TicketTypes", "Attractions");
            entity.Property(e => e.TicketTypeCode).HasColumnName("ticket_type_code");
            entity.Property(e => e.TicketTypeName).HasMaxLength(50).HasColumnName("ticket_type_name");
            entity.Property(e => e.SortOrder).HasColumnName("sort_order");
        });

        // ── 13. 票券標籤對應表 ───────────────────────────────────
        modelBuilder.Entity<AttractionProductTag>(entity =>
        {
            entity.HasKey(e => new { e.ProductId, e.TagId });
            entity.ToTable("AttractionProductTags", "Attractions");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.TagId).HasColumnName("tag_id");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.AttractionProductTags)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_AttractionProductTags_AttractionProducts");

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