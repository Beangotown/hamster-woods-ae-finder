namespace HamsterWoodsApp.GraphQL;

public class GetPurchaseRecordsDto : PagedResultRequestDto
{
    public string? CaAddress { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}