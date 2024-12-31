namespace HamsterWoodsApp.Commons;

public static class BreakHelper
{
    private const long BreakHeight = 146579744; //2024-10-31T00:55:53
    
    public static void CheckBreak(long currentHeight)
    {
        if (BreakHeight <= 0)
        {
            return;
        }

        if (BreakHeight <= currentHeight)
        {
            throw new Exception("Used to wait for node data rollback.");
        }
    }
}