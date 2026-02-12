using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TravelWeb.Models.Travel;

public partial class TravelDBContext : DbContext
{
    public TravelDBContext()
    {
    }

    public TravelDBContext(DbContextOptions<TravelDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcitivityTicket> AcitivityTickets { get; set; }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<ActivityAnalytic> ActivityAnalytics { get; set; }

    public virtual DbSet<ActivityEditLog> ActivityEditLogs { get; set; }

    public virtual DbSet<ActivityImage> ActivityImages { get; set; }

    public virtual DbSet<ActivityMapping> ActivityMappings { get; set; }

    public virtual DbSet<ActivityNotification> ActivityNotifications { get; set; }

    public virtual DbSet<ActivityPublishStatus> ActivityPublishStatuses { get; set; }

    public virtual DbSet<ActivityTicketCategory> ActivityTicketCategories { get; set; }

    public virtual DbSet<ActivityTicketDetail> ActivityTicketDetails { get; set; }

    public virtual DbSet<ActivityTicketDiscount> ActivityTicketDiscounts { get; set; }

    public virtual DbSet<Administrator> Administrators { get; set; }

    public virtual DbSet<Aianalysis> Aianalyses { get; set; }

    public virtual DbSet<AigenerationError> AigenerationErrors { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<ArticleFolder> ArticleFolders { get; set; }

    public virtual DbSet<ArticleLike> ArticleLikes { get; set; }

    public virtual DbSet<ArticleSource> ArticleSources { get; set; }

    public virtual DbSet<ArticleTag> ArticleTags { get; set; }

    public virtual DbSet<Attraction> Attractions { get; set; }

    public virtual DbSet<AttractionProduct> AttractionProducts { get; set; }

    public virtual DbSet<AttractionProductDetail> AttractionProductDetails { get; set; }

    public virtual DbSet<AttractionProductFavorite> AttractionProductFavorites { get; set; }

    public virtual DbSet<AttractionTypeCategory> AttractionTypeCategories { get; set; }

    public virtual DbSet<AttractionTypeMapping> AttractionTypeMappings { get; set; }

    public virtual DbSet<AuditNote> AuditNotes { get; set; }

    public virtual DbSet<Authorization> Authorizations { get; set; }

    public virtual DbSet<Blocked> Blockeds { get; set; }

    public virtual DbSet<CancellationPolicy> CancellationPolicies { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<CommentLike> CommentLikes { get; set; }

    public virtual DbSet<CommentPhoto> CommentPhotos { get; set; }

    public virtual DbSet<ComplaintRecord> ComplaintRecords { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Itinerary> Itineraries { get; set; }

    public virtual DbSet<ItineraryComparison> ItineraryComparisons { get; set; }

    public virtual DbSet<ItineraryItem> ItineraryItems { get; set; }

    public virtual DbSet<ItineraryProductCollection> ItineraryProductCollections { get; set; }

    public virtual DbSet<ItineraryVersion> ItineraryVersions { get; set; }

    public virtual DbSet<Journal> Journals { get; set; }

    public virtual DbSet<JournalElement> JournalElements { get; set; }

    public virtual DbSet<JournalPage> JournalPages { get; set; }

    public virtual DbSet<LogInRecord> LogInRecords { get; set; }

    public virtual DbSet<MemberComplaint> MemberComplaints { get; set; }

    public virtual DbSet<MemberInformation> MemberInformations { get; set; }

    public virtual DbSet<MemberList> MemberLists { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderItemTicket> OrderItemTickets { get; set; }

    public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; }

    public virtual DbSet<PersonalizedRecommendation> PersonalizedRecommendations { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostPhoto> PostPhotos { get; set; }

    public virtual DbSet<ProductInventoryStatus> ProductInventoryStatuses { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<ReportLog> ReportLogs { get; set; }

    public virtual DbSet<Resource> Resources { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    public virtual DbSet<StockInRecord> StockInRecords { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<TagsActivityType> TagsActivityTypes { get; set; }

    public virtual DbSet<TagsList> TagsLists { get; set; }

    public virtual DbSet<TagsRegion> TagsRegions { get; set; }

    public virtual DbSet<TicketCategory> TicketCategories { get; set; }

    public virtual DbSet<TicketType> TicketTypes { get; set; }

    public virtual DbSet<TripItineraryItem> TripItineraryItems { get; set; }

    public virtual DbSet<TripProduct> TripProducts { get; set; }

    public virtual DbSet<TripSchedule> TripSchedules { get; set; }

    public virtual DbSet<UserActivityLog> UserActivityLogs { get; set; }

    public virtual DbSet<UserFavorite> UserFavorites { get; set; }

    public virtual DbSet<UserSearchHistory> UserSearchHistories { get; set; }

    public virtual DbSet<View1> View1s { get; set; }

    public virtual DbSet<ViewArticleHideStatus> ViewArticleHideStatuses { get; set; }

    public virtual DbSet<VwAllProductList> VwAllProductLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=.;database=Travel;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcitivityTicket>(entity =>
        {
            entity.HasKey(e => e.ProductCode).HasName("PK_商品代碼總表");

            entity.ToTable("Acitivity_Tickets", "Activity");

            entity.Property(e => e.ProductCode).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(10);
            entity.Property(e => e.TicketCategoryId).HasColumnName("TicketCategoryID");

            entity.HasOne(d => d.ProductCodeNavigation).WithOne(p => p.AcitivityTicket)
                .HasForeignKey<AcitivityTicket>(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_商品代碼總表_活動商品表細節");

            entity.HasMany(d => d.Discounts).WithMany(p => p.ProductCodes)
                .UsingEntity<Dictionary<string, object>>(
                    "ActivityTicketDiscount1",
                    r => r.HasOne<ActivityTicketDiscount>().WithMany()
                        .HasForeignKey("DiscountId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_商品_折扣對照表_商品折扣表"),
                    l => l.HasOne<AcitivityTicket>().WithMany()
                        .HasForeignKey("ProductCode")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_商品_折扣對照表_商品代碼總表"),
                    j =>
                    {
                        j.HasKey("ProductCode", "DiscountId").HasName("PK_商品_折扣對照表");
                        j.ToTable("ActivityTicket_Discount", "Activity");
                        j.IndexerProperty<string>("ProductCode").HasMaxLength(50);
                        j.IndexerProperty<int>("DiscountId").HasColumnName("DiscountID");
                    });
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK_活動表_1");

            entity.ToTable("Activities", "Activity");

            entity.Property(e => e.ActivityId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ActivityID");
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.UpdateAt).HasColumnType("datetime");

            entity.HasOne(d => d.ActivityNavigation).WithOne(p => p.Activity)
                .HasForeignKey<Activity>(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_活動表_活動熱度分析1");

            entity.HasOne(d => d.Activity1).WithOne(p => p.Activity)
                .HasForeignKey<Activity>(d => d.ActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_活動表_自動化刊登/下架1");

            entity.HasMany(d => d.Regions).WithMany(p => p.Activities)
                .UsingEntity<Dictionary<string, object>>(
                    "ActivityRegion",
                    r => r.HasOne<TagsRegion>().WithMany()
                        .HasForeignKey("RegionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_活動標籤化-區域_標籤_區域表"),
                    l => l.HasOne<Activity>().WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_活動標籤化-區域_活動表"),
                    j =>
                    {
                        j.HasKey("ActivityId", "RegionId").HasName("PK_活動標籤化_區域");
                        j.ToTable("Activity_Region", "Activity");
                        j.IndexerProperty<int>("ActivityId").HasColumnName("ActivityID");
                        j.IndexerProperty<int>("RegionId").HasColumnName("RegionID");
                    });

            entity.HasMany(d => d.Types).WithMany(p => p.Activities)
                .UsingEntity<Dictionary<string, object>>(
                    "ActivityActivityType",
                    r => r.HasOne<TagsActivityType>().WithMany()
                        .HasForeignKey("TypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_活動標籤化-類型_標籤_活動類型表"),
                    l => l.HasOne<Activity>().WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_活動標籤化-類型_活動表"),
                    j =>
                    {
                        j.HasKey("ActivityId", "TypeId").HasName("PK_活動標籤化_類型");
                        j.ToTable("Activity_ActivityTypes", "Activity");
                        j.IndexerProperty<int>("ActivityId").HasColumnName("ActivityID");
                        j.IndexerProperty<int>("TypeId").HasColumnName("TypeID");
                    });
        });

        modelBuilder.Entity<ActivityAnalytic>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK_活動熱度分析_1");

            entity.ToTable("ActivityAnalytics", "Activity");

            entity.Property(e => e.ActivityId)
                .ValueGeneratedNever()
                .HasColumnName("ActivityID");
            entity.Property(e => e.Rating).HasColumnType("decimal(5, 4)");
        });

        modelBuilder.Entity<ActivityEditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK_活動編輯紀錄_1");

            entity.ToTable("ActivityEditLogs", "Activity");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Action).HasMaxLength(10);
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.EditDate).HasColumnType("datetime");

            entity.HasOne(d => d.Activity).WithMany(p => p.ActivityEditLogs)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_活動編輯紀錄_活動表1");
        });

        modelBuilder.Entity<ActivityImage>(entity =>
        {
            entity.HasKey(e => e.ImageSetId).HasName("PK_活動圖片表");

            entity.ToTable("ActivityImages", "Activity");

            entity.Property(e => e.ImageSetId).HasColumnName("ImageSetID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");

            entity.HasOne(d => d.Activity).WithMany(p => p.ActivityImages)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_活動圖片表_活動表");
        });

        modelBuilder.Entity<ActivityMapping>(entity =>
        {
            entity.HasKey(e => e.MappingId).HasName("PK__Activity__8B5781BD04E800C6");

            entity.ToTable("ActivityMapping", "Itinerary");

            entity.Property(e => e.MappingId).HasColumnName("MappingID");
            entity.Property(e => e.ActivityId)
                .HasMaxLength(50)
                .HasColumnName("ActivityID");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ItineraryId).HasColumnName("ItineraryID");

            entity.HasOne(d => d.Itinerary).WithMany(p => p.ActivityMappings)
                .HasForeignKey(d => d.ItineraryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActivityMapping_Itineraries");
        });

        modelBuilder.Entity<ActivityNotification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK_活動通知_1");

            entity.ToTable("ActivityNotification", "Activity");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.NotificaitonType).HasMaxLength(10);
            entity.Property(e => e.SendStatus).HasMaxLength(10);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Activity).WithMany(p => p.ActivityNotifications)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_活動通知_活動表1");
        });

        modelBuilder.Entity<ActivityPublishStatus>(entity =>
        {
            entity.HasKey(e => e.ActivityId).HasName("PK_自動化刊登/下架_1");

            entity.ToTable("ActivityPublishStatus", "Activity");

            entity.Property(e => e.ActivityId)
                .ValueGeneratedNever()
                .HasColumnName("ActivityID");
            entity.Property(e => e.PublishTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(10);
            entity.Property(e => e.UnPublishTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<ActivityTicketCategory>(entity =>
        {
            entity.HasKey(e => e.TicketCategoryId).HasName("PK_票種分類表");

            entity.ToTable("Activity_TicketCategories", "Activity");

            entity.Property(e => e.TicketCategoryId).HasColumnName("TicketCategoryID");
            entity.Property(e => e.CategoryName).HasMaxLength(10);
        });

        modelBuilder.Entity<ActivityTicketDetail>(entity =>
        {
            entity.HasKey(e => e.ProductCode).HasName("PK_活動商品表細節");

            entity.ToTable("Activity_TicketDetails", "Activity");

            entity.Property(e => e.ProductCode).HasMaxLength(50);
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");

            entity.HasOne(d => d.Activity).WithMany(p => p.ActivityTicketDetails)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_活動商品表細節_活動表");
        });

        modelBuilder.Entity<ActivityTicketDiscount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK_商品折扣表");

            entity.ToTable("Activity_TicketDiscounts", "Activity");

            entity.Property(e => e.DiscountId).HasColumnName("DiscountID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.HasKey(e => e.AdminId);

            entity.ToTable("Administrator", "Member");

            entity.Property(e => e.AdminId).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Aianalysis>(entity =>
        {
            entity.HasKey(e => e.AnalysisId).HasName("PK__AIAnalys__5B789DE89E5C3867");

            entity.ToTable("AIAnalyses", "Itinerary");

            entity.Property(e => e.AnalysisId).HasColumnName("AnalysisID");
            entity.Property(e => e.AnalysisTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FatigueIndex).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.FeasibilityScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.PaceBalanceScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.VersionId).HasColumnName("VersionID");

            entity.HasOne(d => d.Version).WithMany(p => p.Aianalyses)
                .HasForeignKey(d => d.VersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AIAnalyse__Versi__57DD0BE4");
        });

        modelBuilder.Entity<AigenerationError>(entity =>
        {
            entity.HasKey(e => e.ErrorId).HasName("PK__AIGenera__358565CACDCE448F");

            entity.ToTable("AIGenerationErrors", "Itinerary");

            entity.Property(e => e.ErrorId).HasColumnName("ErrorID");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ErrorType).HasMaxLength(50);
            entity.Property(e => e.IsConfirmed).HasDefaultValue(false);
            entity.Property(e => e.RelatedItemId).HasColumnName("RelatedItemID");
            entity.Property(e => e.SeverityLevel).HasMaxLength(50);
            entity.Property(e => e.VersionId).HasColumnName("VersionID");

            entity.HasOne(d => d.Version).WithMany(p => p.AigenerationErrors)
                .HasForeignKey(d => d.VersionId)
                .HasConstraintName("FK_AIGenerationErrors_ItineraryVersions");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.ToTable("Article", "Board");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(30);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<ArticleFolder>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ArticleFolder", "Board");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("UserID");

            entity.HasOne(d => d.Article).WithMany()
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleFolder_Article");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleFolder_Member_Information");
        });

        modelBuilder.Entity<ArticleLike>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ArticleLike", "Board");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Article).WithMany()
                .HasForeignKey(d => d.ArticleId)
                .HasConstraintName("FK_ArticleLike_Article");
        });

        modelBuilder.Entity<ArticleSource>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ArticleSources", "Board");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");

            entity.HasOne(d => d.Article).WithMany()
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleSources_Article");
        });

        modelBuilder.Entity<ArticleTag>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ArticleTags", "Board");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.TagId).HasColumnName("TagID");

            entity.HasOne(d => d.Article).WithMany()
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleTags_Article");

            entity.HasOne(d => d.Tag).WithMany()
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleTags_TagsList");
        });

        modelBuilder.Entity<Attraction>(entity =>
        {
            entity.ToTable("Attractions", "Attractions");

            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.ApprovalStatus)
                .HasDefaultValue(0)
                .HasColumnName("approval_status");
            entity.Property(e => e.AreaId)
                .HasMaxLength(50)
                .HasColumnName("area_id");
            entity.Property(e => e.BusinessHours)
                .HasMaxLength(500)
                .HasColumnName("business_hours");
            entity.Property(e => e.ClosedDaysNote)
                .HasMaxLength(200)
                .HasColumnName("closed_days_note");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GooglePlaceId)
                .HasMaxLength(255)
                .HasColumnName("google_place_id");
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(10, 7)")
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(10, 7)")
                .HasColumnName("longitude");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.OpendataId)
                .HasMaxLength(100)
                .HasColumnName("opendata_id");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.TransportInfo).HasColumnName("transport_info");
            entity.Property(e => e.Website)
                .HasMaxLength(500)
                .HasColumnName("website");

            entity.HasOne(d => d.Region).WithMany(p => p.Attractions)
                .HasForeignKey(d => d.RegionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attractions_Tags_Regions");
        });

        modelBuilder.Entity<AttractionProduct>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.ToTable("AttractionProducts", "Attractions");

            entity.HasIndex(e => e.ProductCode, "UQ_AttractionProducts_ProductCode").IsUnique();

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.MaxPurchaseQuantity).HasColumnName("max_purchase_quantity");
            entity.Property(e => e.PolicyId).HasColumnName("policy_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(50)
                .HasColumnName("product_code");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("DRAFT")
                .HasColumnName("status");
            entity.Property(e => e.TicketTypeCode)
                .HasMaxLength(20)
                .HasColumnName("ticket_type_code");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Attraction).WithMany(p => p.AttractionProducts)
                .HasForeignKey(d => d.AttractionId)
                .HasConstraintName("FK_AttractionProducts_Attractions");

            entity.HasMany(d => d.Tags).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "AttractionProductTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_AttractionProductTags_Tags"),
                    l => l.HasOne<AttractionProduct>().WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_AttractionProductTags_AttractionProducts"),
                    j =>
                    {
                        j.HasKey("ProductId", "TagId").HasName("PK__Attracti__332B17DE9A42B909");
                        j.ToTable("AttractionProductTags", "Attractions");
                        j.IndexerProperty<int>("ProductId").HasColumnName("product_id");
                        j.IndexerProperty<int>("TagId").HasColumnName("tag_id");
                    });
        });

        modelBuilder.Entity<AttractionProductDetail>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AttractionProductDetails", "Attractions");

            entity.Property(e => e.ContentDetails).HasColumnName("content_details");
            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_updated_at");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UsageInstructions).HasColumnName("usage_instructions");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_AttractionProductDetails_AttractionProducts");
        });

        modelBuilder.Entity<AttractionProductFavorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK__Attracti__46ACF4CBDF390037");

            entity.ToTable("AttractionProductFavorites", "Attractions");

            entity.HasIndex(e => new { e.UserId, e.ProductId }, "UQ_Favorites_UserProduct").IsUnique();

            entity.Property(e => e.FavoriteId).HasColumnName("favorite_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.AttractionProductFavorites)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_AttractionProductFavorites_AttractionProducts");

            entity.HasOne(d => d.User).WithMany(p => p.AttractionProductFavorites)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AttractionProductFavorites_Member_Information");
        });

        modelBuilder.Entity<AttractionTypeCategory>(entity =>
        {
            entity.HasKey(e => e.AttractionTypeId).HasName("PK__Attracti__61CA9FF14ABB1A87");

            entity.ToTable("AttractionTypeCategories", "Attractions");

            entity.Property(e => e.AttractionTypeId).HasColumnName("attraction_type_id");
            entity.Property(e => e.AttractionTypeName)
                .HasMaxLength(50)
                .HasColumnName("attraction_type_name");
        });

        modelBuilder.Entity<AttractionTypeMapping>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AttractionTypeMappings", "Attractions");

            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.AttractionTypeId).HasColumnName("attraction_type_id");

            entity.HasOne(d => d.Attraction).WithMany()
                .HasForeignKey(d => d.AttractionId)
                .HasConstraintName("FK_AttractionTypeMappings_Attractions");

            entity.HasOne(d => d.AttractionType).WithMany()
                .HasForeignKey(d => d.AttractionTypeId)
                .HasConstraintName("FK_AttractionTypeMappings_AttractionTypeCategories");
        });

        modelBuilder.Entity<AuditNote>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuditNote", "Board");

            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.TargetId).HasColumnName("TargetID");
        });

        modelBuilder.Entity<Authorization>(entity =>
        {
            entity.ToTable("Authorization", "Member");

            entity.Property(e => e.AuthorizationId).ValueGeneratedNever();
            entity.Property(e => e.AdminId).HasMaxLength(50);
            entity.Property(e => e.ExecutedAt).HasColumnType("datetime");
            entity.Property(e => e.MemberCode).HasMaxLength(50);
            entity.Property(e => e.Permission).HasMaxLength(50);
            entity.Property(e => e.Remark).HasMaxLength(255);

            entity.HasOne(d => d.Admin).WithMany(p => p.Authorizations)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Authorization_Administrator");

            entity.HasOne(d => d.MemberCodeNavigation).WithMany(p => p.Authorizations)
                .HasForeignKey(d => d.MemberCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Authorization_Member_List");
        });

        modelBuilder.Entity<Blocked>(entity =>
        {
            entity.HasKey(e => e.MemberId);

            entity.ToTable("blocked", "Member");

            entity.Property(e => e.MemberId).HasMaxLength(50);
            entity.Property(e => e.BlockedId).HasMaxLength(50);
            entity.Property(e => e.Reason).HasMaxLength(100);
        });

        modelBuilder.Entity<CancellationPolicy>(entity =>
        {
            entity.HasKey(e => e.PolicyId).HasName("PK__Cancella__2E1339A4CC83D9F9");

            entity.ToTable("CancellationPolicies", "product");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.PolicyName).HasMaxLength(50);
            entity.Property(e => e.RefundRate).HasColumnType("decimal(3, 2)");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comment", "Board");

            entity.Property(e => e.CommentId)
                .ValueGeneratedNever()
                .HasColumnName("CommentID");
            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.Contents).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Article).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comment_Article");
        });

        modelBuilder.Entity<CommentLike>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CommentLike", "Board");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Comment).WithMany()
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommentLike_Comment");
        });

        modelBuilder.Entity<CommentPhoto>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CommentPhotos", "Board");

            entity.Property(e => e.CommentId).HasColumnName("CommentID");

            entity.HasOne(d => d.Comment).WithMany()
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CommentPhotos_Comment");
        });

        modelBuilder.Entity<ComplaintRecord>(entity =>
        {
            entity.HasKey(e => e.ComplaintId);

            entity.ToTable("Complaint_Record", "Member");

            entity.Property(e => e.ComplaintId).HasMaxLength(50);
            entity.Property(e => e.AdminId).HasMaxLength(50);
            entity.Property(e => e.Compensation).HasMaxLength(255);
            entity.Property(e => e.MemberCode).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Admin).WithMany(p => p.ComplaintRecords)
                .HasForeignKey(d => d.AdminId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Complaint_Record_Administrator");

            entity.HasOne(d => d.MemberCodeNavigation).WithMany(p => p.ComplaintRecords)
                .HasForeignKey(d => d.MemberCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Complaint_Record_Member_List");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Images", "Attractions");

            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.ImagePath)
                .HasMaxLength(255)
                .HasColumnName("image_path");

            entity.HasOne(d => d.Attraction).WithMany()
                .HasForeignKey(d => d.AttractionId)
                .HasConstraintName("FK_Images_Attractions");
        });

        modelBuilder.Entity<Itinerary>(entity =>
        {
            entity.HasKey(e => e.ItineraryId).HasName("PK__Itinerar__361216A6E8732ED0");

            entity.ToTable("Itineraries", "Itinerary");

            entity.Property(e => e.ItineraryId).HasColumnName("ItineraryID");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentStatus).HasMaxLength(50);
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.Introduction).HasMaxLength(500);
            entity.Property(e => e.ItineraryImage).HasMaxLength(1000);
            entity.Property(e => e.ItineraryName).HasMaxLength(200);
            entity.Property(e => e.MemberId).HasMaxLength(50);
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Member).WithMany(p => p.Itineraries)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Itineraries_Member_Information");
        });

        modelBuilder.Entity<ItineraryComparison>(entity =>
        {
            entity.HasKey(e => e.ComparisonId).HasName("PK__Itinerar__6E1F99B7545F294D");

            entity.ToTable("ItineraryComparisons", "Itinerary");

            entity.Property(e => e.ComparisonId).HasColumnName("ComparisonID");
            entity.Property(e => e.ComparedVersionId).HasColumnName("ComparedVersionID");
            entity.Property(e => e.ComparisonTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OriginalVersionId).HasColumnName("OriginalVersionID");

            entity.HasOne(d => d.ComparedVersion).WithMany(p => p.ItineraryComparisonComparedVersions)
                .HasForeignKey(d => d.ComparedVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerary__Compa__58D1301D");

            entity.HasOne(d => d.OriginalVersion).WithMany(p => p.ItineraryComparisonOriginalVersions)
                .HasForeignKey(d => d.OriginalVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerary__Origi__59C55456");
        });

        modelBuilder.Entity<ItineraryItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Itinerar__727E83EBFABCDC81");

            entity.ToTable("ItineraryItems", "Itinerary");

            entity.Property(e => e.ItemId).HasColumnName("ItemID");
            entity.Property(e => e.ActivityId)
                .HasMaxLength(50)
                .HasColumnName("ActivityID");
            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.StartTime).HasColumnType("datetime");
            entity.Property(e => e.VersionId).HasColumnName("VersionID");

            entity.HasOne(d => d.Attraction).WithMany(p => p.ItineraryItems)
                .HasForeignKey(d => d.AttractionId)
                .HasConstraintName("FK_ItineraryItems_Attractions");

            entity.HasOne(d => d.Version).WithMany(p => p.ItineraryItems)
                .HasForeignKey(d => d.VersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerary__Versi__5AB9788F");
        });

        modelBuilder.Entity<ItineraryProductCollection>(entity =>
        {
            entity.HasKey(e => e.FavoriteProductId);

            entity.ToTable("ItineraryProductCollection");

            entity.Property(e => e.MemberId).HasMaxLength(50);

            entity.HasOne(d => d.Member).WithMany(p => p.ItineraryProductCollections)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_ItineraryProductCollection_Member_Information");
        });

        modelBuilder.Entity<ItineraryVersion>(entity =>
        {
            entity.HasKey(e => e.VersionId).HasName("PK__Itinerar__16C6402F82939D88");

            entity.ToTable("ItineraryVersions", "Itinerary");

            entity.Property(e => e.VersionId).HasColumnName("VersionID");
            entity.Property(e => e.CreateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CurrentUsageStatus).HasMaxLength(50);
            entity.Property(e => e.ItineraryId).HasColumnName("ItineraryID");
            entity.Property(e => e.Source).HasMaxLength(50);
            entity.Property(e => e.VersionRemark).HasMaxLength(500);

            entity.HasOne(d => d.Itinerary).WithMany(p => p.ItineraryVersions)
                .HasForeignKey(d => d.ItineraryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Itinerary__Itine__5BAD9CC8");
        });

        modelBuilder.Entity<Journal>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Journal", "Board");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.CoverId).HasColumnName("CoverID");
            entity.Property(e => e.TemplateId).HasColumnName("TemplateID");

            entity.HasOne(d => d.Article).WithMany()
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Journal_Article");
        });

        modelBuilder.Entity<JournalElement>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("JournalElements", "Board");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.ElementId).HasColumnName("ElementID");
            entity.Property(e => e.Zindex).HasColumnName("ZIndex");

            entity.HasOne(d => d.Element).WithMany()
                .HasForeignKey(d => d.ElementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JournalElements_Article");
        });

        modelBuilder.Entity<JournalPage>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("JournalPage", "Board");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.Date).HasDefaultValue(1);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Article).WithMany()
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JournalPage_Article");

            entity.HasOne(d => d.Region).WithMany()
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_JournalPage_Tags_Regions");
        });

        modelBuilder.Entity<LogInRecord>(entity =>
        {
            entity.ToTable("Log_in_record", "Member");

            entity.Property(e => e.LoginRecordId).ValueGeneratedNever();
            entity.Property(e => e.LoginAt).HasColumnType("datetime");
            entity.Property(e => e.MemberCode).HasMaxLength(50);

            entity.HasOne(d => d.MemberCodeNavigation).WithMany(p => p.LogInRecords)
                .HasForeignKey(d => d.MemberCode)
                .HasConstraintName("FK_Log_in_record_Member_List");
        });

        modelBuilder.Entity<MemberComplaint>(entity =>
        {
            entity.HasKey(e => e.ComplaintId);

            entity.ToTable("Member_Complaint", "Member");

            entity.Property(e => e.ComplaintId).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasMaxLength(50);
            entity.Property(e => e.ReplyEmail).HasMaxLength(100);

            entity.HasOne(d => d.Complaint).WithOne(p => p.MemberComplaint)
                .HasForeignKey<MemberComplaint>(d => d.ComplaintId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Member_Complaint_Complaint_Record");

            entity.HasOne(d => d.Member).WithMany(p => p.MemberComplaints)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Member_Complaint_Member_Information");
        });

        modelBuilder.Entity<MemberInformation>(entity =>
        {
            entity.HasKey(e => e.MemberId);

            entity.ToTable("Member_Information", "Member");

            entity.Property(e => e.MemberId).HasMaxLength(50);
            entity.Property(e => e.AvatarUrl).HasMaxLength(255);
            entity.Property(e => e.MemberCode).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.MemberCodeNavigation).WithMany(p => p.MemberInformations)
                .HasForeignKey(d => d.MemberCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Member_Information_Member_List");

            entity.HasOne(d => d.Member).WithOne(p => p.MemberInformation)
                .HasForeignKey<MemberInformation>(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Member_Information_blocked");

            entity.HasMany(d => d.Followeds).WithMany(p => p.Followers)
                .UsingEntity<Dictionary<string, object>>(
                    "MemberFollowing",
                    r => r.HasOne<MemberInformation>().WithMany()
                        .HasForeignKey("FollowedId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Member_Following_Member_Information1"),
                    l => l.HasOne<MemberInformation>().WithMany()
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Member_Following_Member_Information"),
                    j =>
                    {
                        j.HasKey("FollowerId", "FollowedId").HasName("PK_Member_Following_1");
                        j.ToTable("Member_Following", "Member");
                        j.IndexerProperty<string>("FollowerId").HasMaxLength(50);
                        j.IndexerProperty<string>("FollowedId").HasMaxLength(50);
                    });

            entity.HasMany(d => d.Followers).WithMany(p => p.Followeds)
                .UsingEntity<Dictionary<string, object>>(
                    "MemberFollowing",
                    r => r.HasOne<MemberInformation>().WithMany()
                        .HasForeignKey("FollowerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Member_Following_Member_Information"),
                    l => l.HasOne<MemberInformation>().WithMany()
                        .HasForeignKey("FollowedId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Member_Following_Member_Information1"),
                    j =>
                    {
                        j.HasKey("FollowerId", "FollowedId").HasName("PK_Member_Following_1");
                        j.ToTable("Member_Following", "Member");
                        j.IndexerProperty<string>("FollowerId").HasMaxLength(50);
                        j.IndexerProperty<string>("FollowedId").HasMaxLength(50);
                    });
        });

        modelBuilder.Entity<MemberList>(entity =>
        {
            entity.HasKey(e => e.MemberCode);

            entity.ToTable("Member_List", "Member");

            entity.Property(e => e.MemberCode).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCFF0307F3C");

            entity.ToTable("Orders", "product");

            entity.Property(e => e.ContactEmail).HasMaxLength(255);
            entity.Property(e => e.ContactName).HasMaxLength(100);
            entity.Property(e => e.ContactPhone).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerNote).HasMaxLength(500);
            entity.Property(e => e.MemberId).HasMaxLength(50);
            entity.Property(e => e.OrderStatus).HasMaxLength(20);
            entity.Property(e => e.PaymentStatus).HasMaxLength(20);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Member).WithMany(p => p.Orders)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Member_Information");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__OrderIte__57ED0681138B173F");

            entity.ToTable("OrderItems", "product");

            entity.Property(e => e.ProductCode).HasMaxLength(50);
            entity.Property(e => e.ProductNameSnapshot).HasMaxLength(255);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Orders");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK_OrderItems_Schedules");
        });

        modelBuilder.Entity<OrderItemTicket>(entity =>
        {
            entity.HasKey(e => e.OrderItemTicketId).HasName("PK__OrderIte__9F6B10C635839B8B");

            entity.ToTable("OrderItemTickets", "product");

            entity.Property(e => e.TicketNameSnapshot).HasMaxLength(50);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.OrderItem).WithMany(p => p.OrderItemTickets)
                .HasForeignKey(d => d.OrderItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OIT_OrderItems");

            entity.HasOne(d => d.TicketCategory).WithMany(p => p.OrderItemTickets)
                .HasForeignKey(d => d.TicketCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OIT_Tickets");
        });

        modelBuilder.Entity<PaymentTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__PaymentT__55433A6BA97FD419");

            entity.ToTable("PaymentTransactions", "product");

            entity.HasIndex(e => e.ProviderTransactionNo, "UQ__PaymentT__10C5588BD82A9F25").IsUnique();

            entity.Property(e => e.CompletedAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(20);
            entity.Property(e => e.PaymentProvider).HasMaxLength(50);
            entity.Property(e => e.ProviderTransactionNo).HasMaxLength(100);
            entity.Property(e => e.ResponseCode).HasMaxLength(20);
            entity.Property(e => e.TransactionStatus).HasMaxLength(20);

            entity.HasOne(d => d.Order).WithMany(p => p.PaymentTransactions)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payments_Orders");
        });

        modelBuilder.Entity<PersonalizedRecommendation>(entity =>
        {
            entity.HasKey(e => e.RecommendId);

            entity.ToTable("PersonalizedRecommendations", "Activity");

            entity.Property(e => e.RecommendId).HasColumnName("RecommendID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.PreferenceBehavior).HasMaxLength(10);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Activity).WithMany(p => p.PersonalizedRecommendations)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_個人化推薦_活動表");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Post", "Board");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");
            entity.Property(e => e.Contents).HasMaxLength(500);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");

            entity.HasOne(d => d.Article).WithMany()
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Post_Article");

            entity.HasOne(d => d.Region).WithMany()
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_Post_Tags_Regions");
        });

        modelBuilder.Entity<PostPhoto>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("PostPhotos", "Board");

            entity.Property(e => e.ArticleId).HasColumnName("ArticleID");

            entity.HasOne(d => d.Article).WithMany()
                .HasForeignKey(d => d.ArticleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PostPhotos_Article");
        });

        modelBuilder.Entity<ProductInventoryStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ProductInventoryStatus", "Attractions");

            entity.Property(e => e.DailyLimit).HasColumnName("daily_limit");
            entity.Property(e => e.InventoryMode)
                .HasMaxLength(20)
                .HasDefaultValue("UNLIMITED")
                .HasColumnName("inventory_mode");
            entity.Property(e => e.LastUpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("last_updated_at");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.SoldQuantity)
                .HasDefaultValue(0)
                .HasColumnName("sold_quantity");

            entity.HasOne(d => d.Product).WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductInventoryStatus_AttractionProducts");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("PK__Regions__01146BAEAAA24E92");

            entity.ToTable("Regions", "Activity");

            entity.Property(e => e.RegionId)
                .ValueGeneratedNever()
                .HasColumnName("region_id");
            entity.Property(e => e.RegionName)
                .HasMaxLength(50)
                .HasColumnName("region_name");
        });

        modelBuilder.Entity<ReportLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("ReportLog", "Board");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.Reason).HasMaxLength(100);
            entity.Property(e => e.TargetId).HasColumnName("TargetID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<Resource>(entity =>
        {
            entity.HasKey(e => e.ResourceId).HasName("PK__Resource__4ED1816F8DB494E7");

            entity.ToTable("Resources", "product");

            entity.Property(e => e.Latitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.MainImage).HasMaxLength(255);
            entity.Property(e => e.ResourceName).HasMaxLength(255);
            entity.Property(e => e.ShortDescription).HasMaxLength(255);
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Shopping__51BCD7B7F931242E");

            entity.ToTable("ShoppingCart", "product");

            entity.HasIndex(e => new { e.MemberId, e.ProductCode }, "UX_ShoppingCart_User_Schedule").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MemberId).HasMaxLength(50);
            entity.Property(e => e.ProductCode).HasMaxLength(50);

            entity.HasOne(d => d.Member).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ShoppingCart_Member_Information");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.ProductCode)
                .HasConstraintName("FK_Cart_Schedules");

            entity.HasOne(d => d.TicketCategory).WithMany(p => p.ShoppingCarts)
                .HasForeignKey(d => d.TicketCategoryId)
                .HasConstraintName("FK_ShoppingCart_TicketCategories");
        });

        modelBuilder.Entity<StockInRecord>(entity =>
        {
            entity.HasKey(e => e.StockInId).HasName("PK__StockInR__F657737DB034425D");

            entity.ToTable("StockInRecords", "Attractions");

            entity.Property(e => e.StockInId).HasColumnName("stock_in_id");
            entity.Property(e => e.InventoryType)
                .HasMaxLength(20)
                .HasDefaultValue("VIRTUAL")
                .HasColumnName("inventory_type");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(50)
                .HasColumnName("product_code");
            entity.Property(e => e.ProductType)
                .HasMaxLength(20)
                .HasColumnName("product_type");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RemainingStock).HasColumnName("remaining_stock");
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .HasColumnName("remarks");
            entity.Property(e => e.StockInDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("stock_in_date");
            entity.Property(e => e.SupplierName)
                .HasMaxLength(100)
                .HasColumnName("supplier_name");
            entity.Property(e => e.UnitCost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unit_cost");

            entity.HasOne(d => d.ProductCodeNavigation).WithMany(p => p.StockInRecords)
                .HasPrincipalKey(p => p.ProductCode)
                .HasForeignKey(d => d.ProductCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockInRecords_AttractionProducts");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId).HasName("PK__Tags__4296A2B675F200BC");

            entity.ToTable("Tags", "Attractions");

            entity.HasIndex(e => e.TagName, "UQ__Tags__E298655C3A98ABFF").IsUnique();

            entity.Property(e => e.TagId).HasColumnName("tag_id");
            entity.Property(e => e.TagName)
                .HasMaxLength(50)
                .HasColumnName("tag_name");
        });

        modelBuilder.Entity<TagsActivityType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK_標籤_活動類型表_1");

            entity.ToTable("Tags_ActivityTypes", "Activity");

            entity.Property(e => e.TypeId).HasColumnName("TypeID");
            entity.Property(e => e.ActivityType).HasMaxLength(50);
        });

        modelBuilder.Entity<TagsList>(entity =>
        {
            entity.HasKey(e => e.TagId);

            entity.ToTable("TagsList", "Board");

            entity.HasIndex(e => e.TagName, "IX_TagsList").IsUnique();

            entity.Property(e => e.TagId)
                .ValueGeneratedNever()
                .HasColumnName("TagID");
            entity.Property(e => e.TagName).HasMaxLength(10);
        });

        modelBuilder.Entity<TagsRegion>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("PK_標籤_區域表_1");

            entity.ToTable("Tags_Regions", "Activity");

            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.RegionName).HasMaxLength(10);
            entity.Property(e => e.Uid).HasColumnName("UID");

            entity.HasOne(d => d.UidNavigation).WithMany(p => p.InverseUidNavigation)
                .HasForeignKey(d => d.Uid)
                .HasConstraintName("FK_Tags_Regions_Tags_Regions");
        });

        modelBuilder.Entity<TicketCategory>(entity =>
        {
            entity.HasKey(e => e.TicketCategoryId).HasName("PK__TicketCa__C84589E67C04231C");

            entity.ToTable("TicketCategories", "product");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
        });

        modelBuilder.Entity<TicketType>(entity =>
        {
            entity.HasKey(e => e.TicketTypeCode).HasName("PK__TicketTy__427E4A98744E0C99");

            entity.ToTable("TicketTypes", "Attractions");

            entity.Property(e => e.TicketTypeCode)
                .HasMaxLength(20)
                .HasColumnName("ticket_type_code");
            entity.Property(e => e.SortOrder)
                .HasDefaultValue(0)
                .HasColumnName("sort_order");
            entity.Property(e => e.TicketTypeName)
                .HasMaxLength(50)
                .HasColumnName("ticket_type_name");
        });

        modelBuilder.Entity<TripItineraryItem>(entity =>
        {
            entity.HasKey(e => e.ItineraryItemId).HasName("PK__TripItin__1BF8587EE59168B4");

            entity.ToTable("TripItineraryItems", "product");

            entity.Property(e => e.ItineraryItemId).ValueGeneratedNever();

            entity.HasOne(d => d.Resource).WithMany(p => p.TripItineraryItems)
                .HasForeignKey(d => d.ResourceId)
                .HasConstraintName("FK_Itinerary_Resources");

            entity.HasOne(d => d.TripProduct).WithMany(p => p.TripItineraryItems)
                .HasForeignKey(d => d.TripProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripItineraryItems_TripProducts");
        });

        modelBuilder.Entity<TripProduct>(entity =>
        {
            entity.HasKey(e => e.TripProductId).HasName("PK__TripProd__36D9B92A7A23B8AB");

            entity.ToTable("TripProducts", "product");

            entity.Property(e => e.CoverImage)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DisplayPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(255);
            entity.Property(e => e.RegionId).HasColumnName("RegionID");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Policy).WithMany(p => p.TripProducts)
                .HasForeignKey(d => d.PolicyId)
                .HasConstraintName("FK_TripProducts_CancellationPolicies");

            entity.HasOne(d => d.Region).WithMany(p => p.TripProducts)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("FK_TripProducts_Tags_Regions");
        });

        modelBuilder.Entity<TripSchedule>(entity =>
        {
            entity.HasKey(e => e.ProductCode).HasName("PK__TripSche__D559BD2170733C82");

            entity.ToTable("TripSchedules", "product");

            entity.Property(e => e.ProductCode).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SoldQuantity).HasDefaultValue(0);
            entity.Property(e => e.Status).HasMaxLength(20);

            entity.HasOne(d => d.TripProduct).WithMany(p => p.TripSchedules)
                .HasForeignKey(d => d.TripProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TripSchedules_TripProducts");

            entity.HasMany(d => d.TicketCategories).WithMany(p => p.ProductCodes)
                .UsingEntity<Dictionary<string, object>>(
                    "TripAndTicketRelation",
                    r => r.HasOne<TicketCategory>().WithMany()
                        .HasForeignKey("TicketCategoryId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TripAndTicketRelation_TicketCategories"),
                    l => l.HasOne<TripSchedule>().WithMany()
                        .HasForeignKey("ProductCode")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_TripAndTicketRelation_TripSchedules"),
                    j =>
                    {
                        j.HasKey("ProductCode", "TicketCategoryId");
                        j.ToTable("TripAndTicketRelation");
                        j.IndexerProperty<string>("ProductCode").HasMaxLength(50);
                    });
        });

        modelBuilder.Entity<UserActivityLog>(entity =>
        {
            entity.HasKey(e => e.LogId);

            entity.ToTable("UserActivityLog", "Board");

            entity.Property(e => e.LogId)
                .ValueGeneratedNever()
                .HasColumnName("LogID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TargetId).HasColumnName("TargetID");
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PK_喜好收藏");

            entity.ToTable("UserFavorites", "Activity");

            entity.Property(e => e.FavoriteId).HasColumnName("FavoriteID");
            entity.Property(e => e.ActivityId).HasColumnName("ActivityID");
            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("UserID");

            entity.HasOne(d => d.Activity).WithMany(p => p.UserFavorites)
                .HasForeignKey(d => d.ActivityId)
                .HasConstraintName("FK_喜好收藏_活動表1");

            entity.HasOne(d => d.User).WithMany(p => p.UserFavorites)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserFavorites_Member_Information");
        });

        modelBuilder.Entity<UserSearchHistory>(entity =>
        {
            entity.HasKey(e => e.SearchId);

            entity.ToTable("UserSearchHistory", "Board");

            entity.Property(e => e.SearchId)
                .ValueGeneratedNever()
                .HasColumnName("SearchID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Keywords).HasMaxLength(50);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<View1>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_1");

            entity.Property(e => e.AttractionId).HasColumnName("attraction_id");
            entity.Property(e => e.TripProductId).HasMaxLength(50);
        });

        modelBuilder.Entity<ViewArticleHideStatus>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_ArticleHideStatus");

            entity.Property(e => e.Note).HasMaxLength(100);
            entity.Property(e => e.TargetId).HasColumnName("TargetID");
        });

        modelBuilder.Entity<VwAllProductList>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_AllProductList");

            entity.Property(e => e.CategoryName).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(12, 2)");
            entity.Property(e => e.ProductCode).HasMaxLength(50);
            entity.Property(e => e.ProductName).HasMaxLength(255);
            entity.Property(e => e.分類)
                .HasMaxLength(4)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
