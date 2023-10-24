using KimmelTemplate.ApplicationServices;
using KimmelTemplate.Common.Configuration;
using KimmelTemplate.Infrastructure;
using Microsoft.AspNetCore.Localization;
using Serilog;
using System.Globalization;

namespace KimmelTemplate.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors();
            services.AddHttpContextAccessor();

            services.AddInfrastructureModule(Configuration);
            services.AddApplicationServicesModule();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            var serilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext();

            loggerFactory.WithFilter(new FilterLoggerSettings
                {
                    {"Microsoft", LogLevel.Warning},
                    {"System", LogLevel.Warning}
                }).AddSerilog(serilog.CreateLogger());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseApiSecurityHttpHeaders();
            app.UseBlockingContentSecurityPolicyHttpHeader();
            app.RemoveServerHeader();
            app.UseNoCacheHttpHeaders();
            app.UseStrictTransportSecurityHttpHeader(env);
            app.UseHttpsRedirection();

            SetCulture(app);

            app.UseRouting();
            app.UseCors(b => b.WithOrigins(Configuration["AppConfig:ClientUrl"]).AllowAnyHeader().AllowAnyMethod());

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private void SetCulture(IApplicationBuilder app)
        {
            var enUsCulture = new CultureInfo("en-US");
            var localizationOptions = new RequestLocalizationOptions()
            {
                SupportedCultures = new List<CultureInfo>() { enUsCulture },
                SupportedUICultures = new List<CultureInfo>() { enUsCulture },
                DefaultRequestCulture = new RequestCulture(enUsCulture),
                FallBackToParentCultures = false,
                FallBackToParentUICultures = false,
                RequestCultureProviders = null
            };
            app.UseRequestLocalization(localizationOptions);
        }

        public virtual void ConfigureAuth(IServiceCollection services)
        {
            //services.AddMicrosoftIdentityWebApiAuthentication(Configuration, "ApiAzureAdKey");

            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(Policies.ApiReader, policy => policy.RequireScope(Scopes.ExampleScope));
            //});

            //services.AddMvcCore(options =>
            //{
            //    options.Filters.Add(new AuthorizeFilter(Policies.ApiReader));
            //});
        }
    }
}
