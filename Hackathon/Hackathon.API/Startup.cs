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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Hackathon.API
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
                    opt.Password.RequiredLength = 0;
                    opt.Password.RequireLowercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(config =>
            {
                config.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        ctx.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                        return Task.FromResult(0);
                    }
                };
            });

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


            // CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });

            // JWT token configuration
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["tokenManagement:secret"]));

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //what to validate
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        //setup validate data
                        IssuerSigningKey = secret,
                        ValidIssuer = Configuration["tokenManagement:issuer"],
                        ValidAudience = Configuration["tokenManagement:audience"]
                    };
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Hackathon API",
                    Version = "v1",
                    Description = "Web API.",
                    TermsOfService = new Uri("https://policies.google.com/terms?hl=en-US"),
                    Contact = new OpenApiContact()
                    {
                        Name = "Hackathon",
                        Email = "info@hackathon.si",
                        Url = new Uri("http://Hackathon.si/")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "Use under MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme.",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                        },
                        new List<string>()
                    }
                });
            });

            services.AddControllers()
                .AddNewtonsoftJson(options => options.UseMemberCasing());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hackathon.API v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors("default");
            app.UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}