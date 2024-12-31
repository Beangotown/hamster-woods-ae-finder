using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;

namespace HamsterWoodsApp.Processors;

public class TokenIssueProcessor: TokenProcessBase<Issued>
{
    public override async Task ProcessAsync(Issued logEvent, LogEventContext context)
    {
        await SaveUserBalanceAsync(logEvent.Symbol, logEvent.To.ToBase58(), logEvent.Amount,
            context);
    }
}