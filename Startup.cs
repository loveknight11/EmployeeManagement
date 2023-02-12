using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement
{
    public class Startup
    {
        private IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(_config.GetConnectionString("EmployeeDbConntection")));
            services.AddMvc(o => 
            { 
                o.EnableEndpointRouting = false;
                // to allow only authorized users to access any controller 
                var policy = new AuthorizationPolicyBuilder()
                                .RequireAuthenticatedUser()
                                .Build();
                o.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddScoped<IEmployeeRepository, SQLEmployeeRepository>();
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>();
            services.AddAuthentication()
                .AddGoogle(options => {
                options.ClientId = _config["Google:ClientId"];
                options.ClientSecret = _config["Google:ClientSecret"];
            }).AddFacebook(options =>
            {
                options.AppId = _config["Facebook:AppId"];
                options.AppSecret = _config["Facebook:AppSecret"];
                //options.CallbackPath = "https://localhost:44346/signin-facebook";
            });
            services.AddAuthorization(o => {
                o.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
                o.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role"));
                o.AddPolicy("CreateRolePolicy", policy => policy.RequireClaim("Create Role"));
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
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/error/{0}");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();
            //app.Run(async context => {
            //    await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            //});
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapGet("/", async context =>
            //    {
            //        await context.Response.WriteAsync(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            //    });
            //});
        }
    }
}
