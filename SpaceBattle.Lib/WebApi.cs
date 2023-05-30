using CoreWCF;

namespace SpaceBattle.Lib;

[ServiceBehavior(IncludeExceptionDetailInFaults = true)]
public class WebApi : IWebApi
{
    public Contract BodyEcho(Contract param){
        try
        {
            Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Create object by dictionary", param).Execute();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return param;
    }
}
