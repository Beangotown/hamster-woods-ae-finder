using Nest;

namespace HamsterWoodsApp.Entities;

public class TransactionInfoIndex
{
    [Keyword] public string TransactionId { get; set; }

    public long TransactionFee { get; set; }

    public DateTime TriggerTime { get; set; }
}