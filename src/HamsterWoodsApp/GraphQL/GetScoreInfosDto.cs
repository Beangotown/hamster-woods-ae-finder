namespace HamsterWoodsApp.GraphQL;

public class GetScoreInfosDto : PagedResultRequestDto
{
    public DateTime? BeginTime { get; set; }
    public DateTime? EndTime { get; set; }
    public List<string> CaAddressList { get; set; }
}