using System.Linq.Expressions;
using AeFinder.Sdk;
using AeFinder.Sdk.Logging;
using GraphQL;
using HamsterWoodsApp.Commons;
using HamsterWoodsApp.Entities;
using Volo.Abp.ObjectMapping;

namespace HamsterWoodsApp.GraphQL;

public class Query
{
    public static async Task<List<MyEntityDto>> MyEntity(
        [FromServices] IReadOnlyRepository<MyEntity> repository,
        [FromServices] IObjectMapper objectMapper,
        GetMyEntityInput input)
    {
        var queryable = await repository.GetQueryableAsync();

        queryable = queryable.Where(a => a.Metadata.ChainId == input.ChainId);

        if (!input.Address.IsNullOrWhiteSpace())
        {
            queryable = queryable.Where(a => a.Address == input.Address);
        }

        var accounts = queryable.ToList();

        return objectMapper.Map<List<MyEntity>, List<MyEntityDto>>(accounts);
    }

    [Name("getWeekRank")]
    public static async Task<WeekRankResultDto> GetWeekRankAsync(
        [FromServices] IReadOnlyRepository<UserWeekRankIndex> rankWeekUserRepository,
        [FromServices] IObjectMapper objectMapper, GetRankDto getRankDto)
    {
        var rankResultDto = new WeekRankResultDto();
        var queryable = await rankWeekUserRepository.GetQueryableAsync();
        queryable = queryable.Where(a => a.WeekNum == getRankDto.WeekNum);

        var result = queryable.OrderByDescending(t => t.SumScore)
            .ThenBy(f => f.UpdateTime).Skip(0).Take(100).ToList();

        var rank = 0;
        var rankDtos = new List<RankDto>();
        foreach (var item in result)
        {
            var rankDto = objectMapper.Map<UserWeekRankIndex, RankDto>(item);
            rankDto.Rank = ++rank;
            rankDtos.Add(rankDto);
            if (rankDto.CaAddress.Equals(getRankDto.CaAddress))
            {
                rankResultDto.SelfRank = rankDto;
            }
        }

        rankResultDto.RankingList = getRankDto.SkipCount >= rankDtos.Count
            ? new List<RankDto>()
            : rankDtos.Skip(getRankDto.SkipCount).Take(getRankDto.MaxResultCount).ToList();

        if (rankResultDto.SelfRank == null)
        {
            var queryableItem = await rankWeekUserRepository.GetQueryableAsync();
            queryableItem = queryableItem.Where(t => t.CaAddress == getRankDto.CaAddress);
            queryableItem = queryableItem.Where(t => t.WeekNum == getRankDto.WeekNum);
            
            var userWeekRankIndex = queryableItem.FirstOrDefault();
            rankResultDto.SelfRank = ConvertWeekRankDto(objectMapper, getRankDto.CaAddress, userWeekRankIndex);
        }

        return rankResultDto;
    }

    private static RankDto ConvertWeekRankDto(IObjectMapper objectMapper, string caAddress,
        UserWeekRankIndex? userWeekRankIndex)
    {
        if (userWeekRankIndex == null)
        {
            return new RankDto
            {
                CaAddress = caAddress,
                Score = 0,
                Rank = CommonConstants.UserDefaultRank
            };
        }

        return objectMapper.Map<UserWeekRankIndex, RankDto>(userWeekRankIndex);
    }

    [Name("getGameHistory")]
    public static async Task<GameHisResultDto> GetGameHistoryAsync(
        [FromServices] IReadOnlyRepository<GameIndex> gameIndexRepository,
        [FromServices] IReadOnlyRepository<TransactionChargedFeeIndex> transactionChargeFeeRepository,
        [FromServices] IObjectMapper objectMapper, GetGameHisDto getGameHisDto)
    {
        if (getGameHisDto.CaAddress.IsNullOrEmpty())
        {
            return new GameHisResultDto()
            {
                GameList = new List<GameResultDto>()
            };
        }

        var queryable = await gameIndexRepository.GetQueryableAsync();
        queryable = queryable.Where(a => a.CaAddress == getGameHisDto.CaAddress);

        var gameResult = queryable.Skip(getGameHisDto.SkipCount).Take(getGameHisDto.MaxResultCount)
            .OrderByDescending(t => t.Metadata.Block.BlockHeight).ToList();

        if (gameResult.IsNullOrEmpty())
        {
            return new GameHisResultDto()
            {
                GameList = new List<GameResultDto>()
            };
        }

        var transactionIdList = gameResult.Where(game => game.BingoTransactionInfo?.TransactionFee > 0)
            .Select(game => game.BingoTransactionInfo?.TransactionId).ToList();

        var feeQueryable = await transactionChargeFeeRepository.GetQueryableAsync();

        if (!transactionIdList.IsNullOrEmpty())
        {
            feeQueryable = feeQueryable.Where(
                transactionIdList.Select(transId =>
                        (Expression<Func<TransactionChargedFeeIndex, bool>>)(t => t.TransactionId == transId))
                    .Aggregate((prev, next) => prev.Or(next)));
        }

        var transactionChargeFeeResult = feeQueryable.ToList();
        var transactionChargeFeeMap = transactionChargeFeeResult.ToDictionary(
            item => item.TransactionId,
            item => item.ChargingAddress);

        foreach (var gameIndex in gameResult)
        {
            var transactionId = gameIndex.BingoTransactionInfo?.TransactionId;
            var address = string.Empty;
            if (transactionId.IsNullOrEmpty()) continue;

            transactionChargeFeeMap.TryGetValue(transactionId, out address);
            if (!gameIndex.CaAddress.Equals(address))
            {
                gameIndex.BingoTransactionInfo.TransactionFee = 0;
            }
        }

        return new GameHisResultDto()
        {
            GameList = objectMapper.Map<List<GameIndex>, List<GameResultDto>>(gameResult)
        };
    }

    [Name("getGoCount")]
    public static async Task<GameGoCountDto> GetGoCount(
        [FromServices] IReadOnlyRepository<GameIndex> repository,
        GetGoDto getGoDto)
    {
        var queryable = await repository.GetQueryableAsync();
        if (!getGoDto.CaAddressList.IsNullOrEmpty())
        {
            queryable = queryable.Where(
                getGoDto.CaAddressList.Select(address =>
                        (Expression<Func<GameIndex, bool>>)(t => t.CaAddress == address))
                    .Aggregate((prev, next) => prev.Or(next)));
        }

        if (getGoDto.StartTime != null)
        {
            queryable = queryable.Where(t => t.BingoTransactionInfo.TriggerTime >= getGoDto.StartTime);
        }

        if (getGoDto.EndTime != null)
        {
            queryable = queryable.Where(t => t.BingoTransactionInfo.TriggerTime <= getGoDto.EndTime);
        }

        var gameList = new List<GameIndex>();
        var skipCount = getGoDto.SkipCount;
        var count = 0;
        do
        {
            var data = queryable.Skip(skipCount).Take(GetGoDto.MaxMaxResultCount).ToList();
            count = data.Count();
            if (count == 0) break;

            gameList.AddRange(data);
            skipCount += GetGoDto.MaxMaxResultCount;
        } while (count >= GetGoDto.MaxMaxResultCount);

        var goResponse = gameList.GroupBy(g => g.CaAddress)
            .Select(group => new { caAddress = group.Key, Count = group.Count() })
            .Where(x => x.Count >= getGoDto.GoCount);

        return new GameGoCountDto
        {
            GoCount = goResponse.Count()
        };
    }

    [Name("getGameHistoryList")]
    public static async Task<GameHistoryResultDto> GetGameHistoryList(
        [FromServices] IReadOnlyRepository<GameIndex> repository,
        [FromServices] IObjectMapper objectMapper, GetGameHistoryDto getGameHistoryDto)
    {
        var queryable = await repository.GetQueryableAsync();
        if (!getGameHistoryDto.CaAddress.IsNullOrEmpty())
        {
            queryable = queryable.Where(t => t.CaAddress == getGameHistoryDto.CaAddress);
        }

        if (getGameHistoryDto.BeginTime != null)
        {
            queryable = queryable.Where(t => t.BingoTransactionInfo.TriggerTime >= getGameHistoryDto.BeginTime);
        }

        if (getGameHistoryDto.EndTime != null)
        {
            queryable = queryable.Where(t => t.BingoTransactionInfo.TriggerTime <= getGameHistoryDto.EndTime);
        }


        var gameResult = queryable.Skip(getGameHistoryDto.SkipCount).Take(getGameHistoryDto.MaxResultCount);
        return new GameHistoryResultDto()
        {
            GameList = objectMapper.Map<List<GameIndex>, List<GameResultDto>>(gameResult.ToList())
        };
    }


    [Name("getUserBalanceList")]
    public static async Task<List<UserBalanceResultDto>> GetUserBalanceList(
        [FromServices] IReadOnlyRepository<UserBalanceIndex> repository,
        [FromServices] IObjectMapper objectMapper, GetUserBalanceDto userBalanceDto)
    {
        var symbols = userBalanceDto.Symbols;
        if (symbols.IsNullOrEmpty())
        {
            return null;
        }

        if (userBalanceDto.Address.IsNullOrEmpty())
        {
            return new List<UserBalanceResultDto>();
        }

        var queryable = await repository.GetQueryableAsync();
        if (!userBalanceDto.ChainId.IsNullOrEmpty())
            queryable = queryable.Where(t => t.Metadata.ChainId == userBalanceDto.ChainId);

        queryable = queryable.Where(t => t.Address == userBalanceDto.Address);
        queryable = queryable.Where(
            symbols.Select(s =>
                    (Expression<Func<UserBalanceIndex, bool>>)(t => t.Symbol == s))
                .Aggregate((prev, next) => prev.Or(next)));

        return objectMapper.Map<List<UserBalanceIndex>, List<UserBalanceResultDto>>(queryable.ToList());
    }

    [Name("getBuyChanceRecords")]
    public static async Task<PurchaseResultDto> GetBuyChanceRecords(
        [FromServices] IReadOnlyRepository<PurchaseChanceIndex> purchaseIndexRepository,
        [FromServices] IReadOnlyRepository<TransactionChargedFeeIndex> transactionChargeFeeRepository,
        [FromServices] IObjectMapper objectMapper, GetBuyChanceRecordsDto getBuyChanceRecordsDto)
    {
        if (getBuyChanceRecordsDto.CaAddress.IsNullOrEmpty())
        {
            return new PurchaseResultDto()
            {
                BuyChanceList = new List<PurchaseDto>()
            };
        }

        var queryable = await purchaseIndexRepository.GetQueryableAsync();
        queryable = queryable.Where(a => a.CaAddress == getBuyChanceRecordsDto.CaAddress);

        var purchases = queryable.Skip(getBuyChanceRecordsDto.SkipCount).Take(getBuyChanceRecordsDto.MaxResultCount)
            .OrderByDescending(t => t.Metadata.Block.BlockHeight).ToList();

        if (purchases.IsNullOrEmpty())
        {
            return new PurchaseResultDto()
            {
                BuyChanceList = new List<PurchaseDto>()
            };
        }

        var transactionIdList = purchases.Where(game => game.TransactionInfo?.TransactionFee > 0)
            .Select(game => game.TransactionInfo?.TransactionId).ToList();

        var feeQueryable = await transactionChargeFeeRepository.GetQueryableAsync();
        if (!transactionIdList.IsNullOrEmpty())
        {
            feeQueryable = feeQueryable.Where(
                transactionIdList.Select(transId =>
                        (Expression<Func<TransactionChargedFeeIndex, bool>>)(t => t.TransactionId == transId))
                    .Aggregate((prev, next) => prev.Or(next)));
        }

        var transactionChargeFeeResult = feeQueryable.ToList();
        var transactionChargeFeeMap = transactionChargeFeeResult.ToDictionary(
            item => item.TransactionId,
            item => item.ChargingAddress);
        foreach (var item in purchases)
        {
            var transactionId = item.TransactionInfo?.TransactionId;
            var address = string.Empty;
            if (transactionId.IsNullOrEmpty()) continue;

            transactionChargeFeeMap.TryGetValue(transactionId, out address);
            if (!item.CaAddress.Equals(address))
            {
                item.TransactionInfo.TransactionFee = 0;
            }
        }

        return new PurchaseResultDto()
        {
            BuyChanceList = objectMapper.Map<List<PurchaseChanceIndex>, List<PurchaseDto>>(purchases)
        };
    }

    [Name("getWeekRankRecords")]
    public static async Task<UserWeekRankRecordDto> GetWeekRankRecords(
        [FromServices] IReadOnlyRepository<UserWeekRankIndex> repository,
        [FromServices] IObjectMapper objectMapper, GetRankRecordsDto getRankRecordsDto)
    {
        var queryable = await repository.GetQueryableAsync();

        if (getRankRecordsDto.WeekNum is > 0)
        {
            queryable = queryable.Where(t => t.WeekNum == getRankRecordsDto.WeekNum);
        }

        var result = queryable.OrderByDescending(t => t.SumScore).ThenBy(f => f.UpdateTime)
            .Skip(getRankRecordsDto.SkipCount).Take(getRankRecordsDto.MaxResultCount);

        return new UserWeekRankRecordDto
        {
            RankRecordList = objectMapper.Map<List<UserWeekRankIndex>, List<RankRecordDto>>(result.ToList())
        };
    }

    [Name("getUnLockedRecords")]
    public static async Task<UnLockedRecordsDto> GetUnLockedRecords(
        [FromServices] IReadOnlyRepository<UnLockAcornsIndex> repository,
        [FromServices] IObjectMapper objectMapper, GetUnLockedRecordsDto getUnLockedRecordsDto)
    {
        var queryable = await repository.GetQueryableAsync();
        queryable = queryable.Where(t => t.CaAddress == getUnLockedRecordsDto.CaAddress);

        if (getUnLockedRecordsDto.WeekNum.HasValue && getUnLockedRecordsDto.WeekNum > 0)
        {
            queryable = queryable.Where(t => t.WeekNum == getUnLockedRecordsDto.WeekNum);
        }

        var result = queryable.OrderByDescending(t => t.WeekNum).Skip(getUnLockedRecordsDto.SkipCount)
            .Take(getUnLockedRecordsDto.MaxResultCount).ToList();

        return new UnLockedRecordsDto
        {
            UnLockRecordList = objectMapper.Map<List<UnLockAcornsIndex>, List<UnLockRecordDto>>(result)
        };
    }

    [Name("getSelfWeekRank")]
    public static async Task<RankDto> GetSelfWeekRank(
        [FromServices] IReadOnlyRepository<UserWeekRankIndex> repository,
        [FromServices] IObjectMapper objectMapper, GetSelfWeekRankDto getRankDto)
    {
        var queryable = await repository.GetQueryableAsync();
        if (getRankDto.WeekNum.HasValue)
        {
            queryable = queryable.Where(t => t.WeekNum == getRankDto.WeekNum);
        }

        var result = queryable.OrderByDescending(t => t.SumScore).ThenBy(t => t.UpdateTime).Skip(0).Take(1000);

        var userRankIndex = result.FirstOrDefault(t => t.CaAddress == getRankDto.CaAddress);
        if (userRankIndex == null)
        {
            var queryableItem = await repository.GetQueryableAsync();
            queryableItem = queryableItem.Where(t => t.CaAddress == getRankDto.CaAddress);
            if (getRankDto.WeekNum.HasValue)
            {
                queryableItem = queryableItem.Where(t => t.WeekNum == getRankDto.WeekNum);
            }
            
            var userWeekRankIndex = queryableItem.FirstOrDefault();
            return ConvertWeekRankDto(objectMapper, getRankDto.CaAddress, userWeekRankIndex);
        }

        var rank = 0;
        foreach (var item in result)
        {
            if (item.CaAddress == getRankDto.CaAddress)
            {
                userRankIndex.Rank = ++rank;
                break;
            }

            ++rank;
        }

        return objectMapper.Map<UserWeekRankIndex, RankDto>(userRankIndex);
    }

    [Name("getScoreInfos")]
    public static async Task<List<ScoreInfosDto>> GetScoreInfos(
        [FromServices] IReadOnlyRepository<GameIndex> repository,
        GetScoreInfosDto getScoreInfosDto)
    {
        var queryable = await repository.GetQueryableAsync();
        if (getScoreInfosDto.CaAddressList.IsNullOrEmpty() || getScoreInfosDto.CaAddressList.Count > 100)
        {
            return new List<ScoreInfosDto>();
        }

        queryable = queryable.Where(
            getScoreInfosDto.CaAddressList.Select(address =>
                    (Expression<Func<GameIndex, bool>>)(t => t.CaAddress == address))
                .Aggregate((prev, next) => prev.Or(next)));

        if (getScoreInfosDto.BeginTime != null)
        {
            queryable = queryable.Where(t => t.BingoTransactionInfo.TriggerTime >= getScoreInfosDto.BeginTime);
        }

        if (getScoreInfosDto.EndTime != null)
        {
            queryable = queryable.Where(t => t.BingoTransactionInfo.TriggerTime <= getScoreInfosDto.EndTime);
        }

        var gameList = new List<GameIndex>();
        var skipCount = 0;
        var maxResultCount = getScoreInfosDto.MaxResultCount;
        var count = 0;
        do
        {
            queryable = queryable.Skip(skipCount).Take(maxResultCount);
            var data = queryable.ToList();
            count = data.Count;
            if (count == 0) break;

            gameList.AddRange(data);
            skipCount += maxResultCount;
        } while (count >= maxResultCount);

        var resultDto = new List<ScoreInfosDto>();
        var gameGroup = gameList.GroupBy(g => g.CaAddress);
        foreach (var groupInfo in gameGroup)
        {
            resultDto.Add(new ScoreInfosDto()
            {
                CaAddress = groupInfo.Key,
                SumScore = groupInfo.Sum(t => t.Score)
            });
        }

        return resultDto;
    }

    [Name("getHopCount")]
    public static async Task<GetHopCountDto> GetHopCountAsync(
        [FromServices] IReadOnlyRepository<GameIndex> repository,
        GetHopCountRequestDto requestDto)
    {
        var queryable = await repository.GetQueryableAsync();
        if (!requestDto.Address.IsNullOrEmpty())
        {
            queryable = queryable.Where(t => t.CaAddress == requestDto.Address);
        }

        if (requestDto.StartTime != null)
        {
            queryable = queryable.Where(t => t.BingoTransactionInfo.TriggerTime >= requestDto.StartTime);
        }

        if (requestDto.EndTime != null)
        {
            queryable = queryable.Where(t => t.BingoTransactionInfo.TriggerTime <= requestDto.EndTime);
        }

        return new GetHopCountDto
        {
            HopCount = queryable.Count()
        };
    }

    [Name("getPurchaseCount")]
    public static async Task<GetPurchaseCountDto> GetPurchaseCountAsync(
        [FromServices] IReadOnlyRepository<PurchaseChanceIndex> repository,
        GetPurchaseCountRequestDto requestDto)
    {
        var queryable = await repository.GetQueryableAsync();
        if (!requestDto.Address.IsNullOrEmpty())
        {
            queryable = queryable.Where(t => t.CaAddress == requestDto.Address);
        }

        if (requestDto.StartTime != null)
        {
            queryable = queryable.Where(t => t.TransactionInfo.TriggerTime >= requestDto.StartTime);
        }

        if (requestDto.EndTime != null)
        {
            queryable = queryable.Where(t => t.TransactionInfo.TriggerTime <= requestDto.EndTime);
        }

        var result = queryable.Skip(0).Take(1000).ToList();
        return new GetPurchaseCountDto
        {
            PurchaseCount = result.IsNullOrEmpty() ? 0 : result.Sum(t => t.Chance)
        };
    }

    [Name("getPurchaseRecords")]
    public static async Task<PurchaseResultDto> GetPurchaseRecords(
        [FromServices] IReadOnlyRepository<PurchaseChanceIndex> repository,
        [FromServices] IObjectMapper objectMapper, GetPurchaseRecordsDto requestDto)
    {
        var queryable = await repository.GetQueryableAsync();
        if (!requestDto.CaAddress.IsNullOrEmpty())
        {
            queryable = queryable.Where(t => t.CaAddress == requestDto.CaAddress);
        }

        if (requestDto.StartTime != null)
        {
            queryable = queryable.Where(t => t.TransactionInfo.TriggerTime >= requestDto.StartTime);
        }

        if (requestDto.EndTime != null)
        {
            queryable = queryable.Where(t => t.TransactionInfo.TriggerTime <= requestDto.EndTime);
        }

        var result = queryable.Skip(requestDto.SkipCount).Take(requestDto.MaxResultCount).ToList();
        return new PurchaseResultDto()
        {
            BuyChanceList = objectMapper.Map<List<PurchaseChanceIndex>, List<PurchaseDto>>(result)
        };
    }
}