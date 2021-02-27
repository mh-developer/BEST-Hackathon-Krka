using AutoMapper;
using Hackathon.Application.Companies;
using Hackathon.Application.DaliveryPoints;
using Hackathon.Application.Deliveries;
using Hackathon.Application.Shared.Emails;
using Hackathon.Application.Users;
using Hackathon.Application.Warehouses;
using Hackathon.Domain.CompanyAggregate;
using Hackathon.Domain.DeliveryAggregate;
using Hackathon.Domain.UserAggregate;
using Hackathon.Domain.WarehouseAggregate;
using Hackathon.Infrastructure;
using Hackathon.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Hackathon.MVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));


            services.AddIdentity<User, IdentityRole<Guid>>(opt =>
                {
                    opt.Password.RequireDigit = false;
                    opt.Password.RequiredLength = 6;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserMapperProfile());
                mc.AddProfile(new CompanyMapperProfile());
                mc.AddProfile(new DeliveryMapperProfile());
                mc.AddProfile(new DeliveryPointMapperProfile());
                mc.AddProfile(new WarehouseMapperProfile());
            });
            var mapper = mapperConfig.CreateMapper();

            services.AddSingleton(mapper);
            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddHttpContextAccessor();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IDeliveryPointService, DeliveryPointService>();
            services.AddScoped<IWarehouseService, WarehouseService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IDeliveryRepository, DeliveryRepository>();
            services.AddScoped<IDeliveryPointRepository, DeliveryPointRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();

            services.AddControllersWithViews();
            services.AddRazorPages();
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