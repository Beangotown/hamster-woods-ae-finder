using AeFinder.Sdk.Entities;
using Nest;

namespace HamsterWoodsApp.Entities;

public class PurchaseChanceIndex: AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    [Keyword] public string CaAddress { get; set; }
    [Keyword] public string Chainid { get; set; }
    public int Chance { get; set; }
    public long Cost { get; set; }
    public int WeekNum { get; set; }
    public TransactionInfoIndex? TransactionInfo { get; set; }
    public ScoreTokenInfo ScoreTokenInfo { get; set; }
}