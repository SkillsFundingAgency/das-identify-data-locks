using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Client.Configuration;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.LearnerDataMismatches.Web.Infrastructure;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Providers.Api.Client;

namespace SFA.DAS.LearnerDataMismatches.Web
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
            services.AddDbContext<PaymentsDataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("PaymentsDatabase"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
            services.AddScoped<IPaymentsDataContext, PaymentsDataContext>();

            services.AddRazorPages(options => {
                options.Conventions.AllowAnonymousToPage("/index");
                options.Conventions.AuthorizePage("/start");
                options.Conventions.AuthorizePage("/learner");
            });
            services.AddAuthentication(Configuration);
            services.AddAuthorization();
            services.Configure<HtmlHelperOptions>(o => o.ClientValidationEnabled = false);
            RegisterServices(services);
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
                app.UseExceptionHandler("/Error");
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
                endpoints.MapRazorPages();
            });
        }

        private void RegisterServices(IServiceCollection services)
        {
            var commitmentsClientApiConfiguration = new CommitmentsClientApiConfiguration();
            Configuration.GetSection(nameof(CommitmentsClientApiConfiguration)).Bind(commitmentsClientApiConfiguration);
            services.AddSingleton<CommitmentsClientApiConfiguration>(commitmentsClientApiConfiguration);
            services.AddSingleton<ICommitmentsApiClientFactory, CommitmentsApiClientFactory>();
            services.AddTransient<ICommitmentsApiClient>(x => x.GetService<ICommitmentsApiClientFactory>().CreateClient());
            services.AddTransient<ICommitmentsService, CommitmentsService>();

            var accountsApiConfiguration = new AccountApiConfiguration();
            Configuration.GetSection(nameof(AccountApiConfiguration)).Bind(accountsApiConfiguration);
            services.AddSingleton<IAccountApiConfiguration>(accountsApiConfiguration);
            services.AddTransient<IAccountApiClient, AccountApiClient>();
            services.AddTransient<IEmployerService, EmployerService>();

            services.AddTransient<IProviderApiClient>((x) => new ProviderApiClient(Configuration.GetValue<string>("FatApiBaseUri")));
            services.AddTransient<IProviderService, ProviderService>();
            services.AddTransient<IDataLockService, DataLockService>();
            services.AddTransient<LearnerReportProvider>();
        }
    }
}
