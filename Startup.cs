using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cookies
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCookiePolicy();
            app.UseMiddleware<ConsentMiddleware>();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("cookie", async context =>
                {
                    var counter1 = int.Parse(context.Request.Cookies["counter1"] ?? "0") + 1;
                    context.Response.Cookies.Append("counter1", counter1.ToString(), new CookieOptions
                    {
                        MaxAge = TimeSpan.FromMinutes(30),
                        IsEssential = true
                    });

                    var counter2 = int.Parse(context.Request.Cookies["counter2"] ?? "0") + 1;
                    context.Response.Cookies.Append("counter2", counter2.ToString(), new CookieOptions
                    {
                        MaxAge = TimeSpan.FromMinutes(30)
                    });

                    await context.Response.WriteAsync($"counter1:{counter1}, counter: {counter2}");
                });

                endpoints.MapGet("clear",  context =>
                {
                    context.Response.Cookies.Delete("counter1");
                    context.Response.Cookies.Delete("counter2");
                    context.Response.Redirect("/");
                    return Task.CompletedTask;
                });

                endpoints.MapFallback( async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });

            });
        }
    }
}
