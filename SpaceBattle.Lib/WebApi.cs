using CoreWCF;

namespace SpaceBattle.Lib;

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class WebApi : IWebApi
{
    public Contract BodyEcho(Contract param){
        foreach(object obj in param.Registration.Entries)
        {
            Console.WriteLine(obj);
        }
        return param;
    }
}
