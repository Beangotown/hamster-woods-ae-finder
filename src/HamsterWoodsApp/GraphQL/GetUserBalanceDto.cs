namespace HamsterWoodsApp.GraphQL;

public class GetUserBalanceDto
{
    public string? ChainId { get; set; }
    public List<string> Symbols { get; set; }
    public string Address { get; set; }
}