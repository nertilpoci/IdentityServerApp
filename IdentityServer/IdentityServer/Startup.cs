using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer.Data;
using IdentityServer.Services;
using IdentityServer4.Services;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            var migrationsAssembly = typeof(Startup).Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));


            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Tokens.ChangePhoneNumberTokenProvider = "Phone";
                options.Password.RequiredLength = 8; 
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

            var cors = new DefaultCorsPolicyService(null)
            {
                AllowAll = true
            };
            services.AddSingleton<ICorsPolicyService>(cors);


            services.AddSingleton<ICorsPolicyService>(cors);

            services.AddMvc()
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.AuthorizeFolder("/Account/Manage");
                    options.Conventions.AuthorizePage("/Account/Logout");
                });


                 services.AddSingleton<IEmailSender, EmailSender>();

            services.AddIdentityServer()
             .AddDeveloperSigningCredential(false)
             // this adds the config data from DB (clients, resources)
             .AddConfigurationStore(options =>
             {
                 options.ConfigureDbContext = builder =>
                     builder.UseSqlServer(connectionString,
                         sql => sql.MigrationsAssembly(migrationsAssembly));
             })
             // this adds the operational data from DB (codes, tokens, consents)
             .AddOperationalStore(options =>
             {
                 options.ConfigureDbContext = builder =>
                     builder.UseSqlServer(connectionString,
                         sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                 options.TokenCleanupInterval = 30;
             })
             .AddAspNetIdentity<ApplicationUser>();
            services.AddCors();
            services.AddAuthentication()
               .AddGoogle(options =>
               {
                   options.ClientId = "45344529038-jufcmr0ed4co5jl6mk6p9of9894vqtvt.apps.googleusercontent.com";
                   options.ClientSecret = "7hNgrFEkSeNL5yr5cNAa5mGu";
               });
            //.AddFacebook(options=> {
            //    options.AppId = "klasdjfk;alsdj faljdfka;df";
            //    options.AppSecret = "lkasja;lsjdf;alskdfj";
            //});
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.InitializeDatabase();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseIdentityServer();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials());


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
