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

            // Hent connection string fra konfiguration
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Registrer repository og service
            builder.Services.AddScoped<IRoomRepository>(provider => new RoomCollectionRepo(connectionString));
            builder.Services.AddScoped<RoomService>();
            // Add services to the container.
            builder.Services.AddDataProtection()
      .PersistKeysToFileSystem(new DirectoryInfo(@"C:\Dataprotection-Keys"))
      .SetApplicationName("ZealandBookingSystem");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
