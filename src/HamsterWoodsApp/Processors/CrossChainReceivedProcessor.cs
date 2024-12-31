using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;

namespace HamsterWoodsApp.Processors;

public class CrossChainReceivedProcessor : TokenProcessBase<CrossChainReceived>
{
    public override async Task ProcessAsync(CrossChainReceived logEvent, LogEventContext context)
    {
        await SaveUserBalanceAsync(logEvent.Symbol, logEvent.To.ToBase58(), logEvent.Amount,
            context);
    }
}