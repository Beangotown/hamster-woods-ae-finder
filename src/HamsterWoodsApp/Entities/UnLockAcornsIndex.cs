using AeFinder.Sdk.Entities;
using Nest;

namespace HamsterWoodsApp.Entities;

public class UnLockAcornsIndex : AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }

    [Keyword] public string FromAddress { get; set; }
    [Keyword] public string CaAddress { get; set; }
    [Keyword] public string Chainid { get; set; }

    public long Amount { get; set; }
    [Keyword] public string Symbol { get; set; }
    public int Decimals { get; set; } = 8;

    public int WeekNum { get; set; }
    public DateTime BlockTime { get; set; }
    public TransactionInfoIndex TransactionInfo { get; set; }
}