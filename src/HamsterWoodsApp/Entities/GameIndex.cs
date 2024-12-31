using AeFinder.Sdk.Entities;
using Contracts.HamsterWoods;
using Nest;

namespace HamsterWoodsApp.Entities;

public class GameIndex: AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    [Keyword] public string CaAddress { get; set; }
    [Keyword] public string Chainid { get; set; }
    public long PlayBlockHeight { get; set; }
    public bool IsComplete { get; set; }
    public GridType GridType { get; set; }
    public int GridNum { get; set; }
    public long Score { get; set; }
    public int WeekNum { get; set; }
    public bool IsRace { get; set; }
    public long BingoBlockHeight { get; set; }
    public TransactionInfoIndex? PlayTransactionInfo { get; set; }
    public TransactionInfoIndex? BingoTransactionInfo { get; set; }
    public ScoreTokenInfo ScoreTokenInfo { get; set; }
    public long RewardAmount { get; set; }
}

public class ScoreTokenInfo
{
    [Keyword] public string Symbol { get; set; }
    public int Decimals { get; set; }
}