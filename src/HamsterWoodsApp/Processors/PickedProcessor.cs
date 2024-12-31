using AeFinder.Sdk.Processor;
using Contracts.HamsterWoods;
using HamsterWoodsApp.Commons;
using HamsterWoodsApp.Entities;

namespace HamsterWoodsApp.Processors;

public class PickedProcessor : HamsterProcessorBase<Picked>
{
    public override async Task ProcessAsync(Picked logEvent, LogEventContext context)
    {
        BreakHelper.CheckBreak(context.Block.BlockHeight);
        await SaveGameIndexAsync(logEvent, context, logEvent.WeekNum, logEvent.IsRace);
        await SaveRankWeekUserIndexAsync(logEvent, context, logEvent.WeekNum);
    }

    private async Task SaveRankWeekUserIndexAsync(Picked logEvent, LogEventContext context, int weekNum)
    {
        var rankWeekUserId = IdGenerateHelper.GenerateId(context.ChainId, logEvent.PlayerAddress.ToBase58(), weekNum);
        var rankWeekUserIndex = await GetEntityAsync<UserWeekRankIndex>(rankWeekUserId);
        if (rankWeekUserIndex == null)
        {
            rankWeekUserIndex = new UserWeekRankIndex()
            {
                Id = rankWeekUserId,
                WeekNum = weekNum,
                CaAddress = AddressUtil.ToFullAddress(logEvent.PlayerAddress.ToBase58(), context.ChainId),
                UpdateTime = context.Block.BlockTime,
                SumScore = logEvent.Score,
                Rank = CommonConstants.UserDefaultRank,
                IsRace = logEvent.IsRace,
                ScoreTokenInfo = new ScoreTokenInfo
                {
                    Symbol = CommonConstants.ScoreInfo.Symbol,
                    Decimals = CommonConstants.ScoreInfo.Decimals
                }
            };
        }
        else
        {
            rankWeekUserIndex.SumScore += logEvent.Score;
            rankWeekUserIndex.UpdateTime = context.Block.BlockTime;
        }
        
        await SaveEntityAsync(rankWeekUserIndex);
    }

    private async Task SaveGameIndexAsync(Picked eventValue, LogEventContext context,
        int weekNum, bool isRace)
    {
        var feeAmount = GetFeeAmount(context.Transaction.ExtraProperties);
        var gameIndex = new GameIndex
        {
            Id = IdGenerateHelper.GenerateId(context.Block.BlockHash, context.Transaction.TransactionId),
            CaAddress = AddressUtil.ToFullAddress(eventValue.PlayerAddress.ToBase58(), context.ChainId),
            Chainid = context.ChainId,
            WeekNum = weekNum,
            IsRace = isRace,
            BingoBlockHeight = context.Block.BlockHeight,
            BingoTransactionInfo = new TransactionInfoIndex()
            {
                TransactionId = context.Transaction.TransactionId,
                TriggerTime = context.Block.BlockTime,
                TransactionFee = feeAmount
            },
            ScoreTokenInfo = new ScoreTokenInfo
            {
                Symbol = CommonConstants.ScoreInfo.Symbol,
                Decimals = CommonConstants.ScoreInfo.Decimals
            },
            RewardAmount = eventValue.RewardAmount
        };

        ObjectMapper.Map(eventValue, gameIndex);
        await SaveEntityAsync(gameIndex);
    }
}