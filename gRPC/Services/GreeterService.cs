using Grpc.Core;
using gRPC;
using SpaceBattle.Lib;
using System.Collections.Concurrent;
using Hwdtech;
namespace gRPC.Services;

public class GreeterService : Greeter.GreeterBase
{

    private SpaceBattle.Lib.IRouter _router;

    private object scope;

    public GreeterService()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        this.scope = Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"));

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", this.scope).Execute();  
        
        BlockingCollection<SpaceBattle.Lib.ICommand> queue1 = new();
        BlockingCollection<SpaceBattle.Lib.ICommand> queue2 = new();
        BlockingCollection<SpaceBattle.Lib.ICommand> orderQueue1 = new();
        BlockingCollection<SpaceBattle.Lib.ICommand> orderQueue2 = new();
        
        ISender snd1 = new SenderAdapter(orderQueue1);
        ISender snd2 = new SenderAdapter(orderQueue2);
        ISender internalSnd1 = new SenderAdapter(queue1);
        ISender internalSnd2 = new SenderAdapter(queue2);
        

        IReceiver rec1 = new RecieverAdapter(queue1);
        IReceiver rec2 = new RecieverAdapter(queue2);
        IReceiver orderRec1 = new RecieverAdapter(orderQueue1);
        IReceiver orderRec2 = new RecieverAdapter(orderQueue2);

        ServerThread thread1 = new(rec1, orderRec1);
        ServerThread thread2 = new(rec2, orderRec2);

        Dictionary<string, ISender> internalDicts = new(){{"1", internalSnd1}, {"2", internalSnd2}};
        Dictionary<string, ISender> routeDict = new(){{"1", snd1}, {"2", snd2}};

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Message deserialize", (object[] args) => {
            SpaceBattle.Lib.ICommand cmd = (SpaceBattle.Lib.ICommand) args[0];

            return cmd;
        }).Execute(); 

        snd1.Send(new ActionCommand(() => {
            InitiateThreadDependenciesStrategy.Run("1");
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetInternalSenderByThreadId", (object[] args) => {
            return internalDicts[(string)args[0]];
        }).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetOrderSenderByThreadId", (object[] args) => {
            return routeDict[(string)args[0]];
        }).Execute();
        }));

        snd2.Send(new ActionCommand(() => {
            InitiateThreadDependenciesStrategy.Run("2");
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetInternalSenderByThreadId", (object[] args) => {
            return internalDicts[(string)args[0]];
        }).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetOrderSenderByThreadId", (object[] args) => {
            return routeDict[(string)args[0]];
        }).Execute();
        }));

        thread1.Start();
        thread2.Start();

        SpaceBattle.Lib.IRouter router = new DictRouter(routeDict);

        _router = router;
    }

    public override Task<StatusReply> SayHello(gRPCMessage request, ServerCallContext context)
    {
        string status = "";

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", this.scope).Execute();  

        Dictionary<string, object> data = (Dictionary<string, object>) ProtobufMapperStrategy.Run(request.Props);

        if(_router.Route(((string) data["gameid"]).Split('.')[0], data))
        {
            status = "good";
        }
        else{
            status = "bad";
        }

        return Task.FromResult(new StatusReply
        {
            Status = status
        });
    }
}
