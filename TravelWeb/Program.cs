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
builder.Services.AddScoped<IDashBoardService, DashBoardService>();
builder.Services.AddScoped<IItineraryService, ItineraryService>();
builder.Services.AddScoped<IItineraryErrorSevice, ItineraryErrorService>();



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

// Area 路由設定 (必須放在預設路由上方)
app.MapControllerRoute(
    name: "MyAreas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// 原本的預設路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
