using AeFinder.Sdk.Processor;
using AElf.Contracts.MultiToken;
using Google.Protobuf;
using HamsterWoodsApp.Commons;
using HamsterWoodsApp.Entities;
using Portkey.Contracts.CA;

namespace HamsterWoodsApp.Processors;

public class TransactionFeeChargedProcessor : HamsterProcessorBase<TransactionFeeCharged>
{
    private static readonly List<string> MethodNames = new() { "Play", "PurchaseChance" };

    public override string GetContractAddress(string chainId)
    {
        return CommonConstants.TokenContractAddress;
    }

    public override async Task ProcessAsync(TransactionFeeCharged logEvent, LogEventContext context)
    {
        var methodName = GetMethodName(context.Transaction.MethodName, context.Transaction.Params);
        if (!MethodNames.Contains(methodName))
        {
            return;
        }

        var chargeId = IdGenerateHelper.GenerateId(context.ChainId, context.Transaction.TransactionId);
        var transactionFeeCharge = new TransactionChargedFeeIndex()
        {
            Id = chargeId,
            TransactionId = context.Transaction.TransactionId
        };

        ObjectMapper.Map(logEvent, transactionFeeCharge);
        transactionFeeCharge.ChargingAddress =
            AddressUtil.ToFullAddress(logEvent.ChargingAddress.ToBase58(), context.ChainId);

        await SaveEntityAsync(transactionFeeCharge);
    }

    private string GetMethodName(string methodName, string parameter)
    {
        if (methodName != "ManagerForwardCall") return methodName;
        var managerForwardCallInput = ManagerForwardCallInput.Parser.ParseFrom(ByteString.FromBase64(parameter));
        return GetMethodName(managerForwardCallInput.MethodName, managerForwardCallInput.Args.ToBase64());
    }
}