using AeFinder.Sdk.Entities;
using Nest;

namespace HamsterWoodsApp.Entities;

public class UserBalanceIndex :  AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    [Keyword] public string Address { get; set; }
    public long Amount { get; set; }
    [Keyword] public string Symbol { get; set; }
    public DateTime ChangeTime { get; set; }
}