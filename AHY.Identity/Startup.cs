using AHY.Identity.Context;
using AHY.Identity.CustomDescriber;
using AHY.Identity.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace AHY.Identity
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {    
            services.AddIdentity<AppUser, AppRole>(opt =>
            {
                //Default Validasyonlar� �zelle�tirdim.
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 1;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Lockout.MaxFailedAccessAttempts= 3; // Default olarak 5 olan hesap kitlenme hakk�n� 3' e d���rd�m.
                //opt.SignIn.RequireConfirmedPhoneNumber = true; Email onaylanm��sa success sign in olabilirim.
            }).AddErrorDescriber<CustomErrorDescriber>().AddEntityFrameworkStores<AHYContext>(); // Validasyon hatalar�n� t�rk�ele�tirmek i�in Custom Error Describer class'� olu�turdum.

            services.ConfigureApplicationCookie(opt =>
            {
                opt.Cookie.HttpOnly = true;
                opt.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                opt.Cookie.Name = "AHYCookie";
                opt.ExpireTimeSpan = TimeSpan.FromDays(25);
                opt.LoginPath = new PathString("/Home/SignIn");
                opt.AccessDeniedPath = new PathString("/Home/AccessDenied");
            });

            services.AddControllersWithViews();
            services.AddDbContext<AHYContext>(opt =>
            {
                opt.UseSqlServer("server=(localdb)\\mssqllocaldb;database=IdentityDb;integrated security=true");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath="/node_modules",
                FileProvider=new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(),"node_modules"))
            });

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
