using HealthCheck.WebApp.MVC.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString: builder.Configuration.GetConnectionString("DefaultConnection"), name: "Meu banco de dados")
    .AddCheck<SystemMemoryHealthCheck>("Memoria");

builder.Services.AddHealthChecksUI(options =>
    {
        options.SetEvaluationTimeInSeconds(5);
        options.MaximumHistoryEntriesPerEndpoint(10);
        options.AddHealthCheckEndpoint("Exemplo Health Check", "/health");
    })
    .AddInMemoryStorage();

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseHealthChecks("/health", new HealthCheckOptions
{
    Predicate = p => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(options =>
{
    options.UIPath = "/dashboard";
});

app.Run();
