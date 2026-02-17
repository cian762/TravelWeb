using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Activity.Models;
using TravelWeb.Areas.Activity.Models.EFModel;
using TravelWeb.Areas.Activity.Service.ActivityServices;
using TravelWeb.Areas.Activity.Service.IActivityServices;
using TravelWeb.Areas.Attractions.Models;//景點的

using TravelWeb.Areas.BoardManagement.Models.BoardDB;

using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.Itinerary.Models.ItineraryDBModel;
using TravelWeb.Areas.Itinerary.Models.Service;
using TravelWeb.Areas.Itinerary.Repository;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
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



// 註冊 BoardDbContext，並指定使用 SQL Server 以及連接字串
builder.Services.AddDbContext<BoardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Travel")));

//景點連線DI-----------------------------------------------------------------
builder.Services.AddDbContext<AttractionsContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Travel")));
//-----------------------------------------------------------------------------


//ActivityDBcontext �A�ȵ��U 260213_���a�j
builder.Services.AddDbContext<ActivityDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("Travel"))
);

//�[�J Cloudinary ���ݹϧɵ��U 260216_���a�j
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddScoped<IPhotoService, PhotoService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapAreaControllerRoute("app", "Activity", "{controller}/{action}");
// Area 路由設定 (必須放在預設路由上方)
app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// 原本的預設路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
