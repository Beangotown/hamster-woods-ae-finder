using AeFinder.Sdk.Entities;
using Nest;

namespace HamsterWoodsApp.Entities;

public class TransactionChargedFeeIndex : AeFinderEntity, IAeFinderEntity
{
    [Keyword] public override string Id { get; set; }
    [Keyword] public string TransactionId { get; set; }
    [Keyword] public string ChargingAddress { get; set; }
    [Keyword] public string Symbol { get; set; }
    public long Amount { get; set; }
}