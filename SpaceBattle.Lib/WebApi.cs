using CoreWCF;

namespace SpaceBattle.Lib;

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class WebApi : IWebApi
{
    public Contract BodyEcho(Contract param){
        return param;
    }
}
