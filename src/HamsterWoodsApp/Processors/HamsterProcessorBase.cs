using AeFinder.Sdk.Processor;
using AElf.CSharp.Core;
using HamsterWoodsApp.Commons;
using Newtonsoft.Json;
using Volo.Abp.ObjectMapping;

namespace HamsterWoodsApp.Processors;

public abstract class HamsterProcessorBase<TEvent> : LogEventProcessorBase<TEvent> where TEvent : IEvent<TEvent>, new()
{
    protected IObjectMapper ObjectMapper => LazyServiceProvider.LazyGetRequiredService<IObjectMapper>();
    
    public override string GetContractAddress(string chainId)
    {
        return CommonConstants.HamsterWoodsAddress;
    }
    
    protected Dictionary<string, long> GetTransactionFee(Dictionary<string, string> extraProperties)
    {
        var feeMap = new Dictionary<string, long>();
        if (extraProperties.TryGetValue("TransactionFee", out var transactionFee))
        {
            feeMap = JsonConvert.DeserializeObject<Dictionary<string, long>>(transactionFee) ??
                     new Dictionary<string, long>();
        }

        if (extraProperties.TryGetValue("ResourceFee", out var resourceFee))
        {
            var resourceFeeMap = JsonConvert.DeserializeObject<Dictionary<string, long>>(resourceFee) ??
                                 new Dictionary<string, long>();
            foreach (var (symbol, fee) in resourceFeeMap)
            {
                if (feeMap.ContainsKey(symbol))
                {
                    feeMap[symbol] += fee;
                }
                else
                {
                    feeMap[symbol] = fee;
                }
            }
        }

        return feeMap;
    }

    protected long GetFeeAmount(Dictionary<string, string> extraProperties)
    {
        var feeMap = GetTransactionFee(extraProperties);
        if (feeMap.TryGetValue("ELF", out var value))
        {
            return value;
        }

        return 0;
    }
}