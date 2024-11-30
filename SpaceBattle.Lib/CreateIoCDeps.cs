using System.Collections.Concurrent;
using System.Collections.Generic;
using Moq;
using Hwdtech;
using VectorSpaceBattle;
using System;
using System.Threading;
using System.Linq;
using Google.Protobuf.Collections;
namespace SpaceBattle.Lib;


public class Dependencies
{
    public static object Run()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Dictionary<int, ServerThread> GameThreads = new();

        Dictionary<int, ISender> GameSenders = new();

        Dictionary<int, Dictionary<string, IUObject>> GamesObjects = new();

        Dictionary<string, IUObject> game1 = new();

        Mock<IUObject> obj = new();

        Mock<IUObject> _obj = new();

        Queue<SpaceBattle.Lib.ICommand> _queue = new();

        _obj.Setup(o => o.get_property("Velocity")).Returns((object) new Vector(1, 1));

        _obj.Setup(o => o.get_property("Position")).Returns((object) new Vector(1, 1));

        obj.Setup(o => o.get_property("Object")).Returns((object) _obj.Object);

        obj.Setup(o => o.get_property("Queue")).Returns((object) _queue);

        game1.Add("obj123", obj.Object);

        GamesObjects.Add(1, game1);

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get id by thread", (object[] args) => {
            ServerThread thread = (ServerThread)args[0];

            return (object) GameThreads.FirstOrDefault(t => t.Value == thread).Key;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get sender by id", (object[] args) => 
        {
            int id = (int)args[0];
            
            return GameSenders[id];
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get thread by id", (object[] args) => 
        {
            int id = (int)args[0];
            
            return GameThreads[id];
        }).Execute();
        
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create and Start Thread", (object[] args) => 
        {
            if (args.Count() == 2)
            {
                int id = (int)args[0]; 
                Action action = (Action)args[1];
                BlockingCollection<SpaceBattle.Lib.ICommand> q = new();
                BlockingCollection<SpaceBattle.Lib.ICommand> q2 = new();
                ISender sender = new SenderAdapter(q);
                IReceiver receiver = new RecieverAdapter(q);
                IReceiver receiver2 = new RecieverAdapter(q2);

                ServerThread thread = new(receiver, receiver2);

                q.Add(new ActionCommand(action));

                thread.Start();

                GameThreads.Add(id, thread);
                GameSenders.Add(id, sender);

                return thread;
            }
            else{
                int id = (int)args[0]; 
                BlockingCollection<SpaceBattle.Lib.ICommand> q = new();
                BlockingCollection<SpaceBattle.Lib.ICommand> q2 = new();
                ISender sender = new SenderAdapter(q);
                IReceiver receiver = new RecieverAdapter(q);
                IReceiver receiver2 = new RecieverAdapter(q2);
                ServerThread thread = new(receiver, receiver2);

                thread.Start();

                GameThreads.Add(id, thread);
                GameSenders.Add(id, sender);

                return thread;
            }
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Command", (object[] args) => 
        {
            int id = (int)args[0]; 

            ISender sender = IoC.Resolve<ISender>("Get sender by id", id);

            SpaceBattle.Lib.ICommand cmd = (SpaceBattle.Lib.ICommand)args[1];

            sender.Send(cmd);
            return (object) true;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Message deserialize", (object[] args) => {
            SpaceBattle.Lib.ICommand cmd = (SpaceBattle.Lib.ICommand) args[0];

            return cmd;
        }).Execute();     

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Hard Stop Thread", (object[] args) => 
        {
            ISender sender = IoC.Resolve<ISender>("Get sender by id", (int)args[0]);
            if (args.Count() == 2)
            {
                int id = (int)args[0]; 
                Action action = (Action)args[1];

                ServerThread thread = IoC.Resolve<ServerThread>("Get thread by id", id);
                Action act = thread.strategy + action;
                BCPushCommand send = new BCPushCommand(((SenderAdapter)sender).queue, new List<SpaceBattle.Lib.ICommand>(){
                    new UpdateBehaviourCommand(thread, act),
                    new HardStop(IoC.Resolve<ServerThread>("Get thread by id", id))});

                return send;
            }
            else{
                int id = (int)args[0]; 

                ServerThread thread = IoC.Resolve<ServerThread>("Get thread by id", id);
                BCPushCommand send = new BCPushCommand(((SenderAdapter)sender).queue, new List<SpaceBattle.Lib.ICommand>(){
                    new HardStop(IoC.Resolve<ServerThread>("Get thread by id", id))});

                return send;
            }
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Soft Stop Thread", (object[] args) => 
        {
            ISender sender = IoC.Resolve<ISender>("Get sender by id", (int)args[0]);
            if (args.Count() == 2)
            {
                int id = (int)args[0];
                Action action = (Action)args[1];
                ServerThread thread = IoC.Resolve<ServerThread>("Get thread by id", id);

                SoftStop cmd = new SoftStop(thread, action);

                BCPushCommand send = new BCPushCommand(((SenderAdapter)sender).queue, new List<SpaceBattle.Lib.ICommand>(){cmd});

                return send;
            }
            else{
                int id = (int)args[0];
                ServerThread thread = IoC.Resolve<ServerThread>("Get thread by id", id);

                SoftStop cmd = new SoftStop(thread);

                BCPushCommand send = new BCPushCommand(((SenderAdapter)sender).queue, new List<SpaceBattle.Lib.ICommand>(){cmd});

                return send;
            }
        }).Execute();

         Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapters.IUObject.Movable", (object[] args) => 
        {
            MovableAdapter adp = new MovableAdapter(args);
            return adp;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ContiniousMovement.Get.Dependencies", (object[] args) =>
        {
            List<string> deps = new List<string>{"MoveCommand"};
            return deps;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "IoC.MoveCommand", (object[] args) =>
        {
            return (SpaceBattle.Lib.ICommand) new MoveCommand(Hwdtech.IoC.Resolve<IMovable>("Adapters.IUObject.Movable", args));
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get Object by ids", (object[] args) => 
        {
            int GameID = (int) args[0];

            string ObjectID = (string) args[1];

            IUObject obj = GamesObjects[GameID][ObjectID];

            return obj;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Stop Move Command", (object[] args) => 
        {
            IUObject obj = (IUObject) args[0];

            Mock<SpaceBattle.Lib.ICommand> cmd = new();

            return cmd.Object;

        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Start Rotation Command", (object[] args) => 
        {
            IUObject obj = (IUObject) args[0];

            Mock<SpaceBattle.Lib.ICommand> cmd = new();

            return cmd.Object;

        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Shoot Command", (object[] args) => 
        {
            IUObject obj = (IUObject) args[0];

            Mock<SpaceBattle.Lib.ICommand> cmd = new();

            return cmd.Object;

        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Map protobuf to dict", (object[] args) => {
            return ProtobufMapperStrategy.Run((MapField<string, string>) args[0]);
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Deserialize Message to Command", (object[] args) => 
        {

            Dictionary<string, object> MessageContent = (Dictionary<string, object>) args[0];

            string MessageType = (string) MessageContent["type"];

            SpaceBattle.Lib.ICommand cmd = Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Create " + MessageType + " by Message", MessageContent);

            string ThreadID = (string) MessageContent["thread"];

            return ((SpaceBattle.Lib.ICommand)Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", ThreadID, cmd));
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create StartMove by Message", (object[] args) => 
        {
            Dictionary<string, object> MessageContent = (Dictionary<string, object>) args[0];

            IUObject obj = Hwdtech.IoC.Resolve<IUObject>("Get Object by ids", MessageContent["gameid"], MessageContent["objid"]);

            SpaceBattle.Lib.ICommand cmd = IoC.Resolve<SpaceBattle.Lib.ICommand>("IoC.CreateMacro", "ContiniousMovement", obj);

            return cmd;
        }).Execute();

        //Hwdtech.IoC.Resolve<ServerThread>("Create and Start Thread", "2");

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

        return scope;
    }
}
