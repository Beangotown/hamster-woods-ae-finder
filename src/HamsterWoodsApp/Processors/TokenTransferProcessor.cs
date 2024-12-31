using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;
using HamsterWoodsApp.Commons;

namespace HamsterWoodsApp.Processors;

public class TokenTransferProcessor: TokenProcessBase<Transferred>
{
    public override async Task ProcessAsync(Transferred logEvent, LogEventContext context)
    {
        BreakHelper.CheckBreak(context.Block.BlockHeight);
        await SaveUserBalanceAsync(logEvent.Symbol,
            logEvent.From.ToBase58(), -logEvent.Amount, context);
        
        await SaveUserBalanceAsync(logEvent.Symbol, logEvent.To.ToBase58(), logEvent.Amount,
            context);
    }
}