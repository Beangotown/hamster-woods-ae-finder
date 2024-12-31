// using AeFinder.Sdk;
// using Contracts.HamsterWoods;
// using HamsterWoodsApp.Commons;
// using HamsterWoodsApp.Entities;
// using HamsterWoodsApp.GraphQL;
// using HamsterWoodsApp.Processors;
// using Shouldly;
// using Volo.Abp.ObjectMapping;
// using Xunit;
//
// namespace HamsterWoodsIndexer.GraphQL;
//
// public class QueryTest : HamsterWoodsAppTestBase
// {
//     private readonly IObjectMapper _objectMapper;
//     private readonly IReadOnlyRepository<GameIndex> _gameIndexRepository;
//     private readonly IReadOnlyRepository<TransactionChargedFeeIndex> _transactionChargeFeeRepository;
//     private readonly IReadOnlyRepository<PurchaseChanceIndex> _purchaseIndexRepository;
//     private readonly IReadOnlyRepository<UserWeekRankIndex> _rankWeekUserRepository;
//
//     private readonly PickedProcessor _pickedProcessor;
//     private readonly ChancePurchasedProcessor _chancePurchasedProcessor;
//
//     public QueryTest()
//     {
//         _objectMapper = GetRequiredService<IObjectMapper>();
//         _pickedProcessor = GetRequiredService<PickedProcessor>();
//         _chancePurchasedProcessor = GetRequiredService<ChancePurchasedProcessor>();
//
//         _gameIndexRepository = GetRequiredService<IReadOnlyRepository<GameIndex>>();
//         _transactionChargeFeeRepository = GetRequiredService<IReadOnlyRepository<TransactionChargedFeeIndex>>();
//         _purchaseIndexRepository = GetRequiredService<IReadOnlyRepository<PurchaseChanceIndex>>();
//         _rankWeekUserRepository = GetRequiredService<IReadOnlyRepository<UserWeekRankIndex>>();
//     }
//
//     [Fact]
//     public async Task GetGameHistoryAsync_Test()
//     {
//         var picked = new Picked()
//         {
//             PlayerAddress = TestAddress
//         };
//         var logEventContext = GenerateLogEventContext(picked);
//         await _pickedProcessor.ProcessAsync(logEventContext);
//         await SaveDataAsync();
//
//         var resultDto = await Query.GetGameHistoryAsync(_gameIndexRepository, _transactionChargeFeeRepository,
//             _objectMapper,
//             new GetGameHisDto
//             {
//                 CaAddress = AddressUtil.ToFullAddress(TestAddress.ToBase58(), "tDVW"),
//                 MaxResultCount = 1000
//             });
//
//         resultDto.ShouldNotBeNull();
//         resultDto.GameList.ShouldNotBeEmpty();
//         resultDto.GameList.Count.ShouldBe(1);
//     }
//
//     [Fact]
//     public async Task GetBuyChanceRecordsAsync_Test()
//     {
//         var purchased = new ChancePurchased()
//         {
//             PlayerAddress = TestAddress,
//             ChanceCount = 10,
//             AcornsAmount = 100000000,
//             TotalAcorns = 3000000000,
//             TotalChance = 15,
//             WeeklyAcorns = 1000000000
//         };
//
//         var logEventContext = GenerateLogEventContext(purchased);
//         await _chancePurchasedProcessor.ProcessAsync(logEventContext);
//         await SaveDataAsync();
//
//         var resultDto = await Query.GetBuyChanceRecords(_purchaseIndexRepository, _transactionChargeFeeRepository,
//             _objectMapper,
//             new GetBuyChanceRecordsDto
//             {
//                 CaAddress = AddressUtil.ToFullAddress(TestAddress.ToBase58(), "tDVW"),
//                 MaxResultCount = 1000
//             });
//
//         resultDto.ShouldNotBeNull();
//         resultDto.BuyChanceList.ShouldNotBeEmpty();
//         resultDto.BuyChanceList.Count.ShouldBe(1);
//     }
//
//     [Fact]
//     public async Task GetWeekRankAsync_Test()
//     {
//         var picked = new Picked()
//         {
//             PlayerAddress = TestAddress,
//             WeekNum = 1,
//             WeeklyAcorns = 100000000,
//             TotalAcorns = 1000000000,
//             Score = 1
//         };
//         var logEventContext = GenerateLogEventContext(picked);
//         await _pickedProcessor.ProcessAsync(logEventContext);
//         await SaveDataAsync();
//
//         var resultDto = await Query.GetWeekRankAsync(_rankWeekUserRepository,
//             _objectMapper,
//             new GetRankDto()
//             {
//                 CaAddress = AddressUtil.ToFullAddress(TestAddress.ToBase58(), "tDVW"),
//                 WeekNum = 1,
//                 MaxResultCount = 1000
//             });
//
//         resultDto.ShouldNotBeNull();
//         resultDto.RankingList.ShouldNotBeEmpty();
//         resultDto.SelfRank.ShouldNotBeNull();
//         //resultDto.GameList.Count.ShouldBe(1);
//     }
// }