namespace HamsterWoodsApp.GraphQL;

public class GetRankRecordsDto: PagedResultRequestDto
{
    public int? WeekNum { get; set; }
}