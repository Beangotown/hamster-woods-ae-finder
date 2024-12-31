using AeFinder.Sdk.Processor;
using Contracts.HamsterWoods;
using HamsterWoodsApp.Commons;
using HamsterWoodsApp.Entities;

namespace HamsterWoodsApp.Processors;

public class UnLockAcornsProcessor : HamsterProcessorBase<AcornsUnlocked>
{
    public override async Task ProcessAsync(AcornsUnlocked logEvent, LogEventContext context)
    {
        var feeAmount = GetFeeAmount(context.Transaction.ExtraProperties);
        var unLockAcornsIndex = new UnLockAcornsIndex
        {
            Id = IdGenerateHelper.GenerateId(logEvent.To.ToBase58(), logEvent.WeekNum.ToString(),
                context.Transaction.TransactionId),
            CaAddress = AddressUtil.ToFullAddress(logEvent.To.ToBase58(), context.ChainId),
            FromAddress = AddressUtil.ToFullAddress(logEvent.From.ToBase58(), context.ChainId),
            Chainid = context.ChainId,
            WeekNum = logEvent.WeekNum,
            Amount = logEvent.Amount,
            Symbol = logEvent.Symbol,
            BlockTime = context.Block.BlockTime,
            TransactionInfo = new TransactionInfoIndex()
            {
                TransactionId = context.Transaction.TransactionId,
                TriggerTime = context.Block.BlockTime,
                TransactionFee = feeAmount
            }
        };

        await SaveEntityAsync(unLockAcornsIndex);
    }
}