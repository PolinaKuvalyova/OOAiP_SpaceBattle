using Hwdtech;
using SpaceBattle.Lib;
using Moq;
using System.Collections.Concurrent;

namespace Spaceship.IoC.Test.No.Strategies;

public class RoutingTests
{
    [Fact]
    public void MainPositiveRoutingTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        var scope = Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"));

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute(); 

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Message deserialize", (object[] args) => {
            SpaceBattle.Lib.ICommand cmd = (SpaceBattle.Lib.ICommand) args[0];

            return cmd;
        }).Execute(); 


        InitiateThreadDependenciesStrategy.Run("1");

        BlockingCollection<SpaceBattle.Lib.ICommand> queue1 = new();
        BlockingCollection<SpaceBattle.Lib.ICommand> orderQueue1 = new();
        
        ISender snd1 = new SenderAdapter(orderQueue1);
        ISender internalSnd1 = new SenderAdapter(queue1);
        

        IReceiver rec1 = new RecieverAdapter(queue1);
        IReceiver orderRec1 = new RecieverAdapter(orderQueue1);


        Dictionary<string, ISender> internalDicts = new(){{"1", internalSnd1}};
        Dictionary<string, ISender> routeDict = new(){{"1", snd1}};

        SpaceBattle.Lib.IRouter router = new DictRouter(routeDict);

        Dictionary<string, object> ValueDictionary1 = new(){{"type", "StartMove"}, {"gameid", "2.1"}, {"objid", "obj123"}, {"thread", "2"}, {"velocity", 1}};
        Dictionary<string, object> ValueDictionary2 = new(){{"type", "StopMove"}, {"gameid", "2.1"}, {"objid", "obj123"}, {"thread", "2"}};
        Dictionary<string, object> ValueDictionary3 = new(){{"type", "StartRotate"}, {"gameid", "2.1"}, {"objid", "obj123"}, {"thread", "2"}};
        Dictionary<string, object> ValueDictionary4 = new(){{"type", "Shoot"}, {"gameid", "2.1"}, {"objid", "obj123"}, {"thread", "2"}};

        Assert.Empty(orderQueue1);

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetOrderSenderByThreadId", (object[] args) => {
            return routeDict[(string)args[0]];
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetInternalSenderByThreadId", (object[] args) => {
            return internalDicts[(string)args[0]];
        }).Execute();

        Hwdtech.IoC.Resolve<object>("Send Order", new ActionCommand(() => {}));
        router.Route("1", ValueDictionary1);
        router.Route("1", ValueDictionary2);
        router.Route("1", ValueDictionary3);
        router.Route("1", ValueDictionary4);

        Assert.NotEmpty(orderQueue1);


        orderRec1.Receive().Execute();
        orderRec1.Receive().Execute();
        orderRec1.Receive().Execute();
        orderRec1.Receive().Execute();
        orderRec1.Receive().Execute();


        Assert.Empty(orderQueue1);
    }

    [Fact]
    public void EndpointInitTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        var scope = Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"));

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        InitiateThreadDependenciesStrategy.Run("1");

        var newScope = Hwdtech.IoC.Resolve<object>("Scopes.Current");

        Assert.NotEqual(scope, newScope);
    }

    [Fact]
    public void RoutingThrowsTest()
    {
        Mock<ISender> snd = new();

        snd.Setup(s => s.Send(It.IsAny<SpaceBattle.Lib.ICommand>())).Throws<Exception>();
        
        Dictionary<string, ISender> routeDict = new(){{"1", snd.Object}};

        SpaceBattle.Lib.IRouter router = new DictRouter(routeDict);

        Dictionary<string, object> ValueDictionary1 = new(){{"type", "StartMove"}, {"gameid", "2.1"}, {"objid", "obj123"}, {"thread", "2"}, {"velocity", 1}};

        Assert.False(router.Route("1", ValueDictionary1));

    }
}
