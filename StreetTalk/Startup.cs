using System;
using System.IO;
using System.Linq;
using GoogleReCaptcha.V3;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StreetTalk.Data;
using StreetTalk.Middleware;
using StreetTalk.Models;
using StreetTalk.Services;

namespace StreetTalk
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<StreetTalkContext>(options =>
            {
                if (Environment.IsDevelopment())
                {
                    options.UseMySql(Configuration.GetValue<string>("ConnectionStrings:db"), ServerVersion.FromString("8.0.22-mysql"));
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                }
                else
                {
                    options.UseMySql(Configuration.GetValue<string>("ConnectionStrings:db"), ServerVersion.FromString("8.0.22-mysql"));
                }
            });

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddHttpClient<ICaptchaValidator, GoogleReCaptchaValidator>();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddResponseCompression();
            services.AddHttpContextAccessor();
            services.AddTransient<IFileUploadService, FileUploadService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<StreetTalkSignInManager>();
            services.AddRazorPages();
            services.AddIdentity<StreetTalkUser, IdentityRole>
                    (options =>
                    {
                        options.SignIn.RequireConfirmedAccount = true;
                        options.Lockout.AllowedForNewUsers = true;
                        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                        options.Lockout.MaxFailedAccessAttempts = 5;
                    })
                    .AddEntityFrameworkStores<StreetTalkContext>()
                    .AddRoles<IdentityRole>()
                    .AddRoleManager<RoleManager<IdentityRole>>()
                    .AddDefaultTokenProviders();
            
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.LoginPath = "/Identity/Account/Login";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            //Blacklist
            if (!File.Exists("./blacklist.txt"))
                File.Create("./blacklist.txt").Dispose();
            app.UseIpBlacklist(File.ReadLines("./blacklist.txt").ToList());
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}