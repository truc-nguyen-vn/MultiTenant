﻿using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultiTenant.Application.Provider.Tenant;
using MultiTenant.Application.Services.Tenants.User;
using MultiTenant.Application.Validators.Tenants;
using MultiTenant.Data.Contexts;
using MultiTenant.Filter;
using System.IdentityModel.Tokens.Jwt;

namespace MultiTenant
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

            services.AddControllersWithViews();

            services.AddFluentValidation(option =>
            {
                option.RegisterValidatorsFromAssemblyContaining<AccountEditValidator>();
            });

            //get host name
            services.AddHttpContextAccessor();

            services.AddScoped<TenantFilter>();
            services.AddScoped<ModelStateAjaxFilter>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITenantProvider, TenantProvider>();

            services.AddDbContext<MultiTenantContext>();
            services.AddDbContext<TenantContext>((provider, options) =>
            {
                var tenantProvider = provider.GetRequiredService<ITenantProvider>();
                var dbName = tenantProvider.GetSubDomainFromHost();

                options.UseSqlServer($@"Server=DESKTOP-I7EOLFR\SQLEXPRESS;Database={dbName};Trusted_Connection=True;user id=sa1; password=sa123; Integrated Security=false");
            });

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultSignInScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
               .AddCookie("Cookies")
               .AddOpenIdConnect("oidc", "tenant", options =>
               {
                   options.SignInScheme = "Cookies";
                   options.Authority = "https://localhost:5000";

                   options.RequireHttpsMetadata = true;

                   options.ClientId = "tenant";
                   options.ClientSecret = "SuperSecretPassword";
                   options.ResponseType = "code";

                   options.UsePkce = true;
                   options.ResponseMode = "query";
                   options.GetClaimsFromUserInfoEndpoint = true;

                   // options.CallbackPath = "/signin-oidc"; // default redirect URI

                   //options.Scope.Add("oidc"); //default
                   //options.Scope.Add("profile"); //default
                   options.ClaimActions.MapUniqueJsonKey("tenant_id", "tenant_id");

                   options.SaveTokens = true;
               });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=AccountManagement}/{action=Index}/{id?}");
            });
        }
    }


    //public class AppCookieManager : ICookieManager
    //{
    //    public void AppendResponseCookie(HttpContext context, string key, string value, CookieOptions options)
    //    {
    //        context.Response.Cookies.Append(key, value, options);
    //        var subOptions = new CookieOptions()
    //        {
    //            Domain = ".localhost",
    //            SameSite = SameSiteMode.Lax,
    //            Secure = true,
    //            IsEssential = true,
    //            HttpOnly = true
    //        };
    //        context.Response.Cookies.Append(key, value, subOptions);
    //    }

    //    public void DeleteCookie(HttpContext context, string key, CookieOptions options)
    //    {
    //        context.Response.Cookies.Delete(key, options);
    //    }

    //    public string GetRequestCookie(HttpContext context, string key)
    //    {
    //        string val = null;
    //        context.Request.Cookies.TryGetValue(key, out val);
    //        return val;
    //    }
    //}
}
