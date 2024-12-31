using AeFinder.Sdk;

namespace HamsterWoodsApp.GraphQL;

public class HamsterWoodsAppSchema : AppSchema<Query>
{
    public HamsterWoodsAppSchema(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}