using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;

namespace HamsterWoodsApp.Processors;

public class TokenBurnedProcessor : TokenProcessBase<Burned>
{
    public override async Task ProcessAsync(Burned logEvent, LogEventContext context)
    {
        // Burned reduce 
        await SaveUserBalanceAsync(logEvent.Symbol,
            logEvent.Burner.ToBase58(), -logEvent.Amount, context);
    }
}