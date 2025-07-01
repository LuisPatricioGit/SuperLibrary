using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SuperLibrary.Web.Data;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Helper;

namespace SuperLibrary.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();

        // Services
        builder.Services.AddIdentity<User, IdentityRole>(cfg =>
        {
            // TODO: Change Security settings
            cfg.User.RequireUniqueEmail = true;
            cfg.Password.RequireDigit = false;
            cfg.Password.RequiredUniqueChars = 0;
            cfg.Password.RequireLowercase = false;
            cfg.Password.RequireUppercase = false;
            cfg.Password.RequireNonAlphanumeric = false;
            cfg.Password.RequiredLength = 6;
        }).AddEntityFrameworkStores<DataContext>();

        builder.Services.AddDbContext<DataContext>(o =>
        {
            // TODO: Change "LocalConnection" to "OnlineConnection"
            o.UseSqlServer(builder.Configuration.GetConnectionString("LocalConnection"));
        });

        // Seeder
        builder.Services.AddTransient<SeedDb>();

        // Helpers
        builder.Services.AddScoped<IUserHelper, UserHelper>(); 
        builder.Services.AddScoped<IImageHelper, ImageHelper>();
        builder.Services.AddScoped<IConverterHelper, ConverterHelper>();

        // Repositories
        builder.Services.AddScoped<IBookRepository, BookRepository>();

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // Run Seeder
        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<SeedDb>();
            await seeder.SeedAsync();
        }

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

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}