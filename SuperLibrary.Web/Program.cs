using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SuperLibrary.Web.Data;
using SuperLibrary.Web.Data.Entities;
using SuperLibrary.Web.Helper;
using System.Linq; // Add this for .Any()

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
            // Security settings
            cfg.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
            cfg.SignIn.RequireConfirmedEmail = true;
            cfg.User.RequireUniqueEmail = true;
            cfg.Password.RequireDigit = true;
            cfg.Password.RequiredUniqueChars = 1;
            cfg.Password.RequireLowercase = true;
            cfg.Password.RequireUppercase = true;
            cfg.Password.RequireNonAlphanumeric = true;
            cfg.Password.RequiredLength = 6;
        })
                 .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DataContext>();

        builder.Services.AddAuthentication()
            .AddCookie();

        builder.Services.AddDbContext<DataContext>(o =>
        {
            o.UseSqlServer(builder.Configuration.GetConnectionString("OnlineConnection"));
        });

        // Seeder
        builder.Services.AddTransient<SeedDb>();

        // Helpers
        builder.Services.AddScoped<IUserHelper, UserHelper>();
        builder.Services.AddScoped<IConverterHelper, ConverterHelper>();
        builder.Services.AddScoped<IBlobHelper, BlobHelper>();
        builder.Services.AddScoped<IMailHelper, MailHelper>();
        builder.Services.AddScoped<IImageHelper, ImageHelper>();

        // Repositories
        builder.Services.AddScoped<IBookRepository, BookRepository>();
        builder.Services.AddScoped<ILoanRepository, LoanRepository>();

        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/AccessDenied";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });

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
            app.UseExceptionHandler("/Errors/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/Error/{0}");

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