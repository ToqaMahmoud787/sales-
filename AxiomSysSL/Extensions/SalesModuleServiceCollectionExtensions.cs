using AxiomSysBL.IServices;
using AxiomSysBL.IServices.ISalesService;
using AxiomSysBL.Services;
using AxiomSysBL.Services.SalesService;
using AxiomSysSL.Security;

namespace AxiomSysSL.Extensions;

public static class SalesModuleServiceCollectionExtensions
{
    public static IServiceCollection AddSalesModule(this IServiceCollection services)
    {
        services.AddScoped<ISharedService, SharedService>();
        services.AddScoped<ISaDeliveryTermService, SaDeliveryTermService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
