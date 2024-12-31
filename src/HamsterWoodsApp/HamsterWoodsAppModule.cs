using AeFinder.Sdk.Processor;
using GraphQL.Types;
using HamsterWoodsApp.GraphQL;
using HamsterWoodsApp.Processors;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace HamsterWoodsApp;

public class HamsterWoodsAppModule: AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<HamsterWoodsAppModule>(); });
        context.Services.AddSingleton<ISchema, HamsterWoodsAppSchema>();
        
        context.Services.AddSingleton<ILogEventProcessor, PickedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, ChancePurchasedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TransactionFeeChargedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, UnLockAcornsProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, CrossChainReceivedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TokenBurnedProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TokenIssueProcessor>();
        context.Services.AddSingleton<ILogEventProcessor, TokenTransferProcessor>();
    }
}