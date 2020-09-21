using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LandonApi.Filters;
using LandonApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using LandonApi.Services;
using AutoMapper;
using LandonApi.Infrastructure;

namespace LandonApi
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
            services.Configure<HotelInfo>(Configuration.GetSection("Info"));

            services.AddScoped<IRoomService, DefaultRoomService>();

            // Use in-memory database for quick dev and testing 
            // TODO: Swap out for real database in production
            services.AddDbContext<HotelApiDbContext>(
                options => options.UseInMemoryDatabase("landondb"));

            services
                .AddMvc(options=> 
                {
                    options.Filters.Add<JsonExceptionFilter>();
                    options.Filters.Add<RequireHttpsOrCloseAttribute>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });

            //allows any api with different origin (for example http call from https://example.com to api https://api.example.com)
            services.AddCors(options =>
            {
                //for test purposes only, allows any origin
                options.AddPolicy("AllowAnyApp", policy => policy.AllowAnyOrigin());

                //can be used in production
                //options.AddPolicy("AllowAnyApp", policy => policy.WithOrigins("https://example.com"));
            });

            services.AddAutoMapper(
                options => options.AddProfile<MappingProfile>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //Commented as we use RequireHttpsOrCloseAttribute
            //app.UseHttpsRedirection();

            app.UseCors("AllowAnyApp");

            app.UseMvc();
        }
    }
}
