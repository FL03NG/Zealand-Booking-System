using Microsoft.AspNetCore.DataProtection;
using Zealand_Booking_System_Library.Repository;
using Zealand_Booking_System_Library.Service;

namespace Zealand_Booking_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();

            // ---------------------------------------------------
            // Connection String
            // ---------------------------------------------------
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // ---------------------------------------------------
            // Dependency Injection
            // ---------------------------------------------------
            builder.Services.AddScoped<IRoomRepository>(provider => new RoomCollectionRepo(connectionString));
            builder.Services.AddScoped<RoomService>();

            builder.Services.AddScoped<IUserRepository>(provider => new UserCollectionRepo(connectionString));
            builder.Services.AddScoped<UserService>();

            // ---------------------------------------------------
            // Data Protection (beholder dit setup)
            // ---------------------------------------------------
            builder.Services.AddDataProtection()
                   .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Dataprotection-Keys"))
                   .SetApplicationName("ZealandBookingSystem");

            // ---------------------------------------------------
            // Session (bruges til login systemet)
            // ---------------------------------------------------
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // ---------------------------------------------------
            // Middleware pipeline
            // ---------------------------------------------------
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Session før Authorization
            app.UseSession();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}