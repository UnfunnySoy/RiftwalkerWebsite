using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- RATE LIMITING (Security Requirement R.6.3.3) ---
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("DoS-Protection", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Request.Headers["X-Device-Id"].ToString() ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 60,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
});
// ----------------------------------------------------

var app = builder.Build();

// --- NEW: Ensure Database is created automatically ---
using (var scope = app.Services.CreateScope())
{
    var context = new RiftwalkerWebsite.Data.ApplicationDBContext();
    context.Database.EnsureCreated();
}
// ----------------------------------------------------

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable Rate Limiting Middleware (Must be after UseRouting)
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .RequireRateLimiting("DoS-Protection"); // Apply globally to default route mapping

app.Run();
