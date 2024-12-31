using AeFinder.Sdk.Processor;
using Contracts.HamsterWoods;
using HamsterWoodsApp.Commons;
using HamsterWoodsApp.Entities;

namespace HamsterWoodsApp.Processors;

public class ChancePurchasedProcessor : HamsterProcessorBase<ChancePurchased>
{
    public override async Task ProcessAsync(ChancePurchased logEvent, LogEventContext context)
    {
        var feeAmount = GetFeeAmount(context.Transaction.ExtraProperties);
        var index = new PurchaseChanceIndex
        {
            Id = IdGenerateHelper.GenerateId(context.Block.BlockHash, context.Transaction.TransactionId),
            CaAddress = AddressUtil.ToFullAddress(logEvent.PlayerAddress.ToBase58(), context.ChainId),
            Chainid = context.ChainId,
            Chance = logEvent.ChanceCount,
            Cost = logEvent.AcornsAmount,
            TransactionInfo = new TransactionInfoIndex
            {
                TransactionId = context.Transaction.TransactionId,
                TriggerTime = context.Block.BlockTime,
                TransactionFee = feeAmount
            },
            ScoreTokenInfo = new ScoreTokenInfo
            {
                Symbol = CommonConstants.ScoreInfo.Symbol,
                Decimals = CommonConstants.ScoreInfo.Decimals
            }
        };

        await SaveEntityAsync(index);
    }
}