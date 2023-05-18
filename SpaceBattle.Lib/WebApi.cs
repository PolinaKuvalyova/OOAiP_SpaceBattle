using CoreWCF;

namespace SpaceBattle.Lib;

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class WebApi : IWebApi
{
    public ExampleContract BodyEcho(ExampleContract param){
        return param;
    }
}
