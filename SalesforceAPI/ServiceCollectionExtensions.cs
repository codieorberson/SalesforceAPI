using Microsoft.Extensions.DependencyInjection;
using SalesforceAPI.Factories;
using SalesforceAPI.Factories.Interfaces;
using SalesforceAPI.Helpers;
using SalesforceAPI.Helpers.Interfaces;
using SalesforceAPI.Services;
using SalesforceAPI.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using SalesforceAPI.Triggers;

namespace SalesforceAPI
{
    public static class ServiceCollectionExtensions
    {
        public static void SetUpSalesforceAPIServices(this IServiceCollection services)
        {
            services.AddHostedService<PicklistsTrigger>();

            services.AddScoped<IRequestFactory, RequestFactory>();

            services.AddSingleton<IJsonHelper, JsonHelper>();
            services.AddSingleton<IAccessTokenService, AccessTokenService>();
            services.AddSingleton<ISalesforceAPIService, SalesforceAPIService>();
            services.AddSingleton<IContactCRUDService, ContactCRUDService>();
            services.AddSingleton<IPicklistsService, PicklistsService>();


        }
    }
}
