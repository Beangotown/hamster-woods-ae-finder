namespace HamsterWoodsApp.Commons;

public static class CommonConstants
{
    public const int UserDefaultRank = -1;

    public const string HamsterWoodsAddress = "m39bMdjpA74Pv7pyA4zn8w6mhz182KpcrtFAnwWCiFmcihNYE";
    public const string TokenContractAddress = "ASh2Wt7nSEmYqnGxPPzp4pnVDU4uhj1XW9Se5VeZcX2UDdyjx";

    public static ScoreInfo ScoreInfo = new()
    {
        Symbol = "ACORNS",
        Decimals = 8
    };
}

public class ScoreInfo
{
    public string Symbol { get; set; }
    public int Decimals { get; set; }
}