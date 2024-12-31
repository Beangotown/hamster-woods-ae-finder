namespace HamsterWoodsApp.GraphQL;

public class GetUnLockedRecordsDto: PagedResultRequestDto
{
    public string CaAddress { get; set; }
    public int? WeekNum { get; set; }
}