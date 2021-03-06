using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SignalIRServerTest.Hubs;
using SignalIRServerTest.Models;
using SignalIRServerTest.Models.Context;
using SignalIRServerTest.Services;

using System;

using IUserIdProvider = Microsoft.AspNetCore.SignalR.IUserIdProvider;

namespace SignalIRServerTest
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
            services.AddControllers();

            services.AddSignalR();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = Authentification.Issuer,
                        ValidateAudience = true,
                        ValidAudience = Authentification.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = Authentification.GetSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                });

            services.AddScoped<Hash>();
            services.AddScoped<ChatHub>();
            services.AddScoped<UnitOfWork>();
            services.AddScoped<EducationContext>();
            services.AddSingleton<IUserIdProvider, UserProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("~/Chat", options =>
                {
                    options.LongPolling.PollTimeout = TimeSpan.FromMinutes(1);
                    options.Transports = HttpTransportType.LongPolling | HttpTransportType.WebSockets;
                });
            });
        }
    }
}
