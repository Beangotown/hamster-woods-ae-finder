using AElf.Contracts.MultiToken;
using AutoMapper;
using Contracts.HamsterWoods;
using HamsterWoodsApp.Entities;
using HamsterWoodsApp.GraphQL;

namespace HamsterWoodsApp;

public class HamsterWoodsAppProfile : Profile
{
    public HamsterWoodsAppProfile()
    {
        CreateMap<MyEntity, MyEntityDto>();
        CreateMap<GameIndex, GameResultDto>().ForMember(destination => destination.TranscationFee,
                opt => opt.MapFrom(source => source.BingoTransactionInfo.TransactionFee))
            .ForMember(dest => dest.Decimals, source => source.MapFrom(f => f.ScoreTokenInfo.Decimals));
        
        CreateMap<TransactionInfoIndex, TransactionInfoDto>();
        CreateMap<Picked, GameIndex>();
        
        CreateMap<PurchaseChanceIndex, PurchaseDto>().ForMember(destination => destination.TranscationFee,
                opt => opt.MapFrom(source => source.TransactionInfo.TransactionFee))
            .ForMember(dest => dest.Symbol, source => source.MapFrom(f => f.ScoreTokenInfo.Symbol))
            .ForMember(dest => dest.Decimals, source => source.MapFrom(f => f.ScoreTokenInfo.Decimals));
        
        CreateMap<UserWeekRankIndex, RankDto>().ForMember(destination => destination.Score,
            opt => opt.MapFrom(source => source.SumScore));

        CreateMap<UserWeekRankIndex, RankRecordDto>()
            .ForMember(dest => dest.Symbol, source => source.MapFrom(f => f.ScoreTokenInfo.Symbol))
            .ForMember(dest => dest.Decimals, source => source.MapFrom(f => f.ScoreTokenInfo.Decimals));
        
        CreateMap<UserBalanceIndex, UserBalanceResultDto>();
        CreateMap<TransactionFeeCharged, TransactionChargedFeeIndex>();
        CreateMap<UnLockAcornsIndex, UnLockRecordDto>();
    }
}