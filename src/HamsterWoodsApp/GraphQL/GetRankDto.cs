namespace HamsterWoodsApp.GraphQL;

public class GetRankDto: PagedResultRequestDto
{
    public string CaAddress { get; set; }
    public int WeekNum { get; set; }
}