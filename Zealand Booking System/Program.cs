using Microsoft.AspNetCore.DataProtection;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// ---------------------------------------------------
// Connection string fra appsettings.json
// ---------------------------------------------------
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ---------------------------------------------------
// Repositories og services
// ---------------------------------------------------
builder.Services.AddScoped<IRoomRepository>(provider => new RoomCollectionRepo(connectionString));
builder.Services.AddScoped<RoomService>();

builder.Services.AddScoped<IUserRepository>(provider => new UserCollectionRepo(connectionString));
builder.Services.AddScoped<UserService>();

builder.Services.AddScoped<INotificationRepository>(provider => new NotificationCollectionRepo(connectionString));
builder.Services.AddScoped<NotificationService>();

// ---------------------------------------------------
// Data Protection
// ---------------------------------------------------
builder.Services.AddDataProtection()
       .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Dataprotection-Keys"))
       .SetApplicationName("ZealandBookingSystem");

// ---------------------------------------------------
// Session
// ---------------------------------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Logout
app.MapPost("/logout", (HttpContext context) =>
{
    context.Session.Clear();
    return Results.Redirect("/Index");
});

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();
app.Run();