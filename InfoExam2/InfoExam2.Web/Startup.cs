using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfoExam2.Core;
using InfoExam2.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace InfoExam2.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddScoped<ApplicationDbContext>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin",
                    policy =>
                    {
                        policy.RequireClaim("Role", bool.TrueString);
                    });
            });
            
            using (var applicationDbContext = new ApplicationDbContext())
            {
                applicationDbContext.Users.Add(new User()
                {
                    Login = "admin",
                    Password = "admin",
                    Admin = true
                });
                applicationDbContext.Dishes.AddRange(new[]
                {
                    new Dish
                    {
                        Cost = 1, Name = "first"
                    }, new Dish
                    {
                        Cost = 2, Name = "second"
                    }
                });
                applicationDbContext.Restaraunts.Add(new Restaraunt
                {
                    Name = "rest"
                });
                applicationDbContext.Orders.Add(new Order
                {
                    UserId = applicationDbContext.Users.First().Id
                });
                applicationDbContext.OrderItems.Add(new OrderItem()
                {
                    Dish = applicationDbContext.Dishes.First(),
                    OrderId = applicationDbContext.Orders.First().Id
                });
                applicationDbContext.PromoCodes.Add(new PromoCode()
                {
                    Code = "Discounthere",
                    Discount = 10.0,
                    StoreDeadLine = DateTime.Now.AddDays(7),
                    UseLimit = 100
                });
                applicationDbContext.SaveChanges();
            }
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
        }
    }
}
