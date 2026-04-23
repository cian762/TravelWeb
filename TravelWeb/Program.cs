using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Activity.Models;
using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Activity.Repository.ActivityRepositories;
using TravelWeb.Areas.Activity.Repository.IActivityRepositories;
using TravelWeb.Areas.Activity.Service.ActivityServices;
using TravelWeb.Areas.Activity.Service.IActivityServices;
using TravelWeb.Areas.Attractions.Models;
using TravelWeb.Areas.BoardManagement.Models.BoardDB;
using TravelWeb.Areas.BoardManagement.Models.IService;
using TravelWeb.Areas.BoardManagement.Models.Service;
using TravelWeb.Areas.Itinerary.Models.ItineraryDBModel;
using TravelWeb.Areas.Itinerary.Models.Service;
using TravelWeb.Areas.Itinerary.Repository;
using TravelWeb.Areas.TripProduct.Models;
using TravelWeb.Areas.TripProduct.Services.Implementation;
using TravelWeb.Areas.TripProduct.Services.InterSer;
using TravelWeb.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// ==========================================
// 🔥 1. 在 builder.Build() 之前，註冊 Session 服務
// ==========================================
builder.Services.AddDistributedMemoryCache(); // Session 需要用到記憶體快取
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // 設定 Session 閒置多久會過期 (通常設 30 分鐘)
    options.Cookie.HttpOnly = true; // 提高安全性
    options.Cookie.IsEssential = true;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontEnd", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7276",
            "https://taiwanstory.site")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});


#region DI服務註冊
//行程商品連線DI
builder.Services.AddDbContext<TripDbContext>(O => O.UseSqlServer(builder.Configuration.GetConnectionString("Travel")));
//行程商品主頁用DI
builder.Services.AddScoped<ITripproducts, Tripproducts>();
//行程細項連線用DI
builder.Services.AddScoped<ITripItineraryItem, STripItineraryItem>();
//行程檔期連線用DI
builder.Services.AddScoped<ITripSchedules, STripSchedules>();
//訂單用連線DI
builder.Services.AddScoped<IOrder, SOrder>();


builder.Services.AddDbContext<TravelContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Travel")));
builder.Services.AddScoped(typeof(IItineraryGenericRepository<>), typeof(ItineraryRepository<>));
builder.Services.AddScoped<IItineraryGenericRepository<Itinerary>, ItineraryRepository<Itinerary>>();
builder.Services.AddScoped<IItineraryGenericRepository<ItineraryVersion>, ItineraryRepository<ItineraryVersion>>();
builder.Services.AddScoped<IItineraryGenericRepository<Aianalysis>, ItineraryRepository<Aianalysis>>();
builder.Services.AddScoped<IItineraryGenericRepository<AigenerationError>, ItineraryRepository<AigenerationError>>();
builder.Services.AddScoped<IItineraryGenericRepository<ItineraryComparison>, ItineraryRepository<ItineraryComparison>>();
builder.Services.AddScoped<IItineraryGenericRepository<TravelWeb.Areas.Itinerary.Models.ItineraryDBModel.Member_Information>, ItineraryRepository<TravelWeb.Areas.Itinerary.Models.ItineraryDBModel.Member_Information>>();
builder.Services.AddScoped<IDashBoardService, DashBoardService>();
builder.Services.AddScoped<IItineraryService, ItineraryService>();
builder.Services.AddScoped<IItineraryErrorSevice, ItineraryErrorService>();
builder.Services.AddScoped<IItineraryCompareService, ItineraryCompareService>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("Allow5500", p =>
    {
        p.WithOrigins("http://127.0.0.1:5500").AllowAnyHeader().AllowAnyMethod();
    });
});

// 註冊 BoardDbContext，並指定使用 SQL Server 以及連接字串
builder.Services.AddDbContext<BoardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Travel")));
builder.Services.AddScoped<INoteService, NoteService>();

//景點連線DI-----------------------------------------------------------------
builder.Services.AddDbContext<AttractionsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Travel")));
//-----------------------------------------------------------------------------


//ActivityDBcontext 服務註冊
builder.Services.AddDbContext<ActivityDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Travel"))
);

//Member
builder.Services.AddDbContext<MemberSystemContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("Travel")));

//Cloudinary 服務註冊
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinaryGFC"));
builder.Services.AddScoped<IPhotoService, PhotoService>();
#endregion



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowFrontEnd");

// ==========================================
// 🔥 2. 啟用 Session 中介軟體 (位置非常重要！)
// 必須放在 app.UseRouting() 之後，app.UseAuthorization() 與 app.MapControllerRoute() 之前！
// ==========================================
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("Allow5500");

app.MapAreaControllerRoute("app", "Activity", "{controller}/{action}");

app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// 沒有 Area 的路由印設
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
