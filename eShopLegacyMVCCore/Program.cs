
using eShopLegacyMVC.Models;
using eShopLegacyMVC.Services;
using eShopLegacy.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSystemWebAdapters()
    // we have to add information about what our session object will look like for serialisation purposes
    .AddJsonSessionSerializer(options =>
    {
        options.RegisterKey<string>("MachineName");
        options.RegisterKey<DateTime>("SessionStartTime");
        options.RegisterKey<SessionDemoModel>("DemoItem");
    })
    .AddRemoteAppClient(options =>
    {
        options.RemoteAppUrl = new(builder.Configuration["ProxyTo"]); // same setting as YARP proxy
        options.ApiKey = builder.Configuration["RemoteAppApiKey"]; // we have to set up api key here for the client and in .net framework app for the server
    }) // we want to read session from the .net framework app
    .AddSessionClient();

builder.Services.AddHttpForwarder();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICatalogService, CatalogService>();
// This db context is added differently as we still using old EF not the EF Core
builder.Services.AddScoped<CatalogDBContext>();
builder.Services.AddSingleton<CatalogItemHiLoGenerator>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseSystemWebAdapters();

app.MapForwarder("/{**catch-all}", app.Configuration["ProxyTo"]).Add(static builder => ((RouteEndpointBuilder)builder).Order = int.MaxValue);

app.MapControllerRoute("Default", "{controller=Catalog}/{action=Index}/{id?}")
    .RequireSystemWebAdapterSession();

app.MapDefaultControllerRoute();

app.Run();
