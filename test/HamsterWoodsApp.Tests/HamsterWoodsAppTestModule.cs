using AeFinder.App.TestBase;
using HamsterWoodsApp;
using HamsterWoodsApp.Processors;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace HamsterWoodsIndexer;

[DependsOn(
    typeof(AeFinderAppTestBaseModule),
    typeof(HamsterWoodsAppModule))]
public class HamsterWoodsAppTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AeFinderAppEntityOptions>(options => { options.AddTypes<HamsterWoodsAppModule>(); });
        
        // Add your Processors.
        // context.Services.AddSingleton<MyLogEventProcessor>();
        context.Services.AddSingleton<PickedProcessor>();
        context.Services.AddSingleton<ChancePurchasedProcessor>();
    }
}