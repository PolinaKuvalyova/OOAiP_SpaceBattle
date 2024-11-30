using Hwdtech;
using System.Collections.Generic;
using Moq;
namespace SpaceBattle.Lib;
using VectorSpaceBattle;

public class InitiateThreadDependenciesStrategy{

    public static void Run(string id){
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Dictionary<string, Dictionary<string, IUObject>> GamesObjects = new();

        Mock<IUObject> obj = new();

        Mock<IUObject> _obj = new();

        Dictionary<string, IUObject> game1 = new();

        Queue<SpaceBattle.Lib.ICommand> _queue = new();

        _obj.Setup(o => o.get_property("Velocity")).Returns((object) new Vector(1, 1));

        _obj.Setup(o => o.get_property("Position")).Returns((object) new Vector(1, 1));

        obj.Setup(o => o.get_property("Object")).Returns((object) _obj.Object);

        obj.Setup(o => o.get_property("Queue")).Returns((object) _queue);

        game1.Add("obj123", obj.Object);

        GamesObjects.Add("2.1", game1);
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CurrentGameId", (object[] args) => {
                return id;
        }).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get Object by ids", (object[] args) => 
        {
            string GameID = (string) args[0];
            string ObjectID = (string) args[1];
            IUObject obj = GamesObjects[GameID][ObjectID];
            return obj;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Message deserialize", (object[] args) => {
            SpaceBattle.Lib.ICommand cmd = (SpaceBattle.Lib.ICommand) args[0];
            return cmd;
        }).Execute(); 

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Command", (object[] args) => {
            SpaceBattle.Lib.ICommand cmd = (SpaceBattle.Lib.ICommand) args[0];
            Hwdtech.IoC.Resolve<ISender>("GetInternalSenderByThreadId", Hwdtech.IoC.Resolve<string>("CurrentGameId")).Send(cmd);
            return (object)true;//todo: fix
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Order", (object[] args) => {
            SpaceBattle.Lib.ICommand cmd = (SpaceBattle.Lib.ICommand) args[0];
            Hwdtech.IoC.Resolve<ISender>("GetOrderSenderByThreadId", Hwdtech.IoC.Resolve<string>("CurrentGameId")).Send(cmd);
            return (object)true; //todo: fix
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapters.IUObject.Movable", (object[] args) => 
        {
            MovableAdapter adp = new MovableAdapter(args);
            return adp;
        }).Execute();

         Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "IoC.MoveCommand", (object[] args) =>
        {
            return (SpaceBattle.Lib.ICommand) new MoveCommand(Hwdtech.IoC.Resolve<IMovable>("Adapters.IUObject.Movable", args));
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ContiniousMovement.Get.Dependencies", (object[] args) =>
        {
            List<string> deps = new List<string>{"MoveCommand"};
            return deps;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Stop Move Command", (object[] args) => 
        {
            Mock<SpaceBattle.Lib.ICommand> cmd = new();
            return cmd.Object;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Start Rotation Command", (object[] args) => 
        {
            Mock<SpaceBattle.Lib.ICommand> cmd = new();
            return cmd.Object;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Shoot Command", (object[] args) => 
        {
            Mock<SpaceBattle.Lib.ICommand> cmd = new();
            
            return cmd.Object;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create StartMove by Message", (object[] args) => 
        {
            Dictionary<string, object> MessageContent = (Dictionary<string, object>) args[0];
            IUObject obj = Hwdtech.IoC.Resolve<IUObject>("Get Object by ids", MessageContent["gameid"], MessageContent["objid"]);
            SpaceBattle.Lib.ICommand cmd = IoC.Resolve<SpaceBattle.Lib.ICommand>("IoC.CreateMacro", "ContiniousMovement", obj);
            return cmd;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create StopMove by Message", (object[] args) => 
        {
            Dictionary<string, object> MessageContent = (Dictionary<string, object>) args[0];
            IUObject obj = Hwdtech.IoC.Resolve<IUObject>("Get Object by ids", MessageContent["gameid"], MessageContent["objid"]);
            SpaceBattle.Lib.ICommand cmd = Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Stop Move Command", obj);
            return cmd;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create StartRotate by Message", (object[] args) => 
        {
            Dictionary<string, object> MessageContent = (Dictionary<string, object>) args[0];
            IUObject obj = Hwdtech.IoC.Resolve<IUObject>("Get Object by ids", MessageContent["gameid"], MessageContent["objid"]);
            SpaceBattle.Lib.ICommand cmd = Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Start Rotation Command", obj);
            return cmd;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create Shoot by Message", (object[] args) => 
        {
            Dictionary<string, object> MessageContent = (Dictionary<string, object>) args[0];
            IUObject obj = Hwdtech.IoC.Resolve<IUObject>("Get Object by ids", MessageContent["gameid"], MessageContent["objid"]);
            SpaceBattle.Lib.ICommand cmd = Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Shoot Command", obj);
            return cmd;
        }).Execute();
    }
}