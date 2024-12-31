using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;

namespace HamsterWoodsApp.Processors;

public class TokenTransferProcessor: TokenProcessBase<Transferred>
{
    public override async Task ProcessAsync(Transferred logEvent, LogEventContext context)
    {
        await SaveUserBalanceAsync(logEvent.Symbol,
            logEvent.From.ToBase58(), -logEvent.Amount, context);
        
        await SaveUserBalanceAsync(logEvent.Symbol, logEvent.To.ToBase58(), logEvent.Amount,
            context);
    }
}