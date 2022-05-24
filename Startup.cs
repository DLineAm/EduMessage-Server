using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SignalIRServerTest.Hubs;
using SignalIRServerTest.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SignalIRServerTest.Services;
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
