using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using TravelWeb.Areas.TripProduct.Models;
using TravelWeb.Areas.TripProduct.Services.Implementation;
using TravelWeb.Areas.TripProduct.Services.InterSer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//行程商品連線DI
builder.Services.AddDbContext<TripDbContext>(O => O.UseSqlServer(builder.Configuration.GetConnectionString("Travel")));
//行程商品主頁用DI
builder.Services.AddScoped<ITripproducts, Tripproducts>();
//行程細項連線用DI
builder.Services.AddScoped<ITripItineraryItem, STripItineraryItem>();

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
// Area 路由設定 (行程商品)
app.MapControllerRoute(
     name: "Trip",
     pattern: "{area:exists}/{controler=Trip}/{action=index}/{id?}"
    );

// 原本的預設路由
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
