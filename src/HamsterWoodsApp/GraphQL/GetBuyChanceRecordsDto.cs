namespace HamsterWoodsApp.GraphQL;

public class GetBuyChanceRecordsDto: PagedResultRequestDto
{
    public string CaAddress { get; set; }
}