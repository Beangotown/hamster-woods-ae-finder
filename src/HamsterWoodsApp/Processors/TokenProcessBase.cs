using AeFinder.Sdk.Processor;
using AElf.CSharp.Core;
using HamsterWoodsApp.Commons;
using HamsterWoodsApp.Entities;
using Volo.Abp.ObjectMapping;

namespace HamsterWoodsApp.Processors;

public abstract class TokenProcessBase<TEvent> : LogEventProcessorBase<TEvent> where TEvent : IEvent<TEvent>, new()
{
    private const string HamsterPassCollectionSymbol = "HAMSTERPASS-";
    private const string AcornsSymbol = "ACORNS";
    protected IObjectMapper ObjectMapper => LazyServiceProvider.LazyGetRequiredService<IObjectMapper>();
    
    public override string GetContractAddress(string chainId)
    {
        return CommonConstants.TokenContractAddress;
    }

    protected async Task SaveUserBalanceAsync(string symbol, string address, long amount, LogEventContext context)
    {
        if (symbol.IsNullOrWhiteSpace() ||
            address.IsNullOrWhiteSpace() ||
            (!symbol.StartsWith(HamsterPassCollectionSymbol) && symbol != AcornsSymbol))
        {
            return;
        }

        var userBalanceId = IdGenerateHelper.GetUserBalanceId(address, context.ChainId, symbol);
        var userBalanceIndex =  await GetEntityAsync<UserBalanceIndex>(userBalanceId);
        if (userBalanceIndex == null)
        {
            userBalanceIndex = new UserBalanceIndex
            {
                Id = userBalanceId,
                Address = address,
                Amount = amount,
                Symbol = symbol,
                ChangeTime = context.Block.BlockTime
            };
        }
        else
        {
            userBalanceIndex.Amount += amount;
            userBalanceIndex.ChangeTime = context.Block.BlockTime;
        }
        
        await SaveEntityAsync(userBalanceIndex);
    }
}