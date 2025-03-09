using Placement_Portal;
using Placement_Portal.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".Placement_Portal.Session"; // Unique session cookie name
    options.IdleTimeout = TimeSpan.FromMinutes(30);   // Session timeout duration
    options.Cookie.HttpOnly = true;                   // Prevent client-side script access
    options.Cookie.IsEssential = true;                // Ensure session persists even if tracking is disabled
});
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor(); // Required for accessing HttpContext
builder.Services.AddControllersWithViews(); // Add MVC controllers and views
builder.Services.AddHttpClient<AuthService>();
builder.Services.AddSession();
var app = builder.Build();

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Enforce HTTPS
}

app.UseCors(builder =>
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Enable session support
app.UseAuthorization();
app.UseAuthentication();
app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"
          );

// Define default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=UserLogin}/{id?}"); // Redirect to login page by default


app.Run();
