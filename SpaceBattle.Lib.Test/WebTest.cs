using Hwdtech;
using VectorSpaceBattle;
using Moq;
using System.Collections.Concurrent;
using SpaceBattle.Lib;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using Newtonsoft;

namespace SpaceBattle.Lib.Test;

public class WebTest
{
    [Fact]
    public object IoCInit()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Dictionary<int, ISender> dictionarySend = new();
        Dictionary<int, ServerThread> dictionaryThread = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create And Start Thread", 
        (object[] args) => 
        {
            Mock<IReceiver> receiver= new();
            Mock<ISender> sender = new();
            BlockingCollection<ICommand> queue = new BlockingCollection<ICommand>();

            receiver.Setup(r => r.Receive()).Returns(() => queue.Take());
            receiver.Setup(r => r.IsEmpty()).Returns(() => queue.Count == 0);

            if(args.Count() == 2)
            {
                Action ac = (Action) args[1];
                ActionCommand action = new(ac);
                queue.Add(action);
            }

            sender.Setup(s => s.Send(It.IsAny<ICommand>())).Callback<ICommand>((command => queue.Add(command)));

            ServerThread thread = new ServerThread(receiver.Object);
            thread.Execute();

            int id = (int) args[0];
            dictionarySend.Add(id, sender.Object);
            dictionaryThread.Add(id, thread);
            return thread;

        }).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapters.IUObject.IMovable", 
        (object[] args) => 
        {
            MovableAdapter adapter = new MovableAdapter(args);
            return adapter;
        }).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Command", 
        (object[] args) => 
        {
            int id = (int) args[0];
            ICommand cmd = (ICommand) args[1];
            ISender send = dictionarySend[id];

            
            return new ActionCommand(() => {send.Send(cmd);});
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Hard Stop The Thread", 
        (object[] args) => 
        {
            int id = (int) args[0];
            Action action = () => {};
            ActionCommand sendHardStop = new ActionCommand(() => {});
            if(args.Count() == 2)
            {
                Action ac = (Action) args[1];

                ServerThread thread_ = Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", id);
                UpdateBehaviourCommand updateBeh= new(thread_, thread_.strategy + ac);
                action = () => {
                    Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", id, updateBeh).Execute();};
            }
            HardStop hardStop = new(dictionaryThread[id]);

            sendHardStop = new ActionCommand(action + (() => {
                    Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", id, hardStop).Execute();
                }));
            return sendHardStop;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Soft Stop The Thread", 
        (object[] args) => 
        {
            int id = (int) args[0];
            ISender sender = dictionarySend[id];
            ActionCommand sendSoftStop = new ActionCommand(() => {});

            if(args.Count() == 2)
            {
                Action ac = (Action) args[1];
                ActionCommand action = new(ac);
                SoftStop softStop = new(dictionaryThread[id], ac);
                sendSoftStop = new ActionCommand(() => {
                    Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", id, softStop).Execute();
                });
            }
            else{
                SoftStop softStop = new(dictionaryThread[id]);
                sendSoftStop = new ActionCommand(() => {
                    Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", id, softStop).Execute();
                });
            }
            return (SpaceBattle.Lib.ICommand)sendSoftStop;
        }).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get Thread by id", 
        (object[] args) => 
        {
            int id = (int) args[0];
            return dictionaryThread[id];
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get Send by id", 
        (object[] args) => 
        {
            int id = (int) args[0];
            return dictionarySend[id];
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get id by thread", 
        (object[] args) => 
        {
            ServerThread ac = (ServerThread) args[0];
            var id = dictionaryThread.FirstOrDefault(x => x.Value == ac).Key;
            
            return (object) id;
        }
        ).Execute();

        return scope;
    }


    [Fact]
    public void StrategyTest()
    {
        var scope = IoCInit();
        
        Dictionary<int, Queue<SpaceBattle.Lib.ICommand>> dictionaryQueue = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get object by id", 
        (object[] args) => 
        {
            int id = (int) args[0];
            return dictionaryQueue[id];
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CreateMoveCommandContinious", 
        (object[] args) => 
        {
            Dictionary<string, object> dict = (Dictionary<string, object>) args[0];
            object obj = dict["object"];
            Vector velocity = (Vector) dict["velocity"];
            Queue<SpaceBattle.Lib.ICommand> queue = (Queue<SpaceBattle.Lib.ICommand>) dict["Game.Queue"];

            Mock<IUObject> uob = new();

            uob.Setup(x => x.get_property("object")).Returns((object) uob.Object);
            uob.Setup(x => x.get_property("queue")).Returns((object) queue);
            uob.Setup(x => x.get_property("velocity")).Returns((object) new Vector(5, 5));
            ICommand command = new StartMoveCommand(uob.Object);

            return command;
        }).Execute();

        Dictionary<string, string> d = new(){
            {"MoveCommand", "CreateMoveCommandContinious"}, 
            {"StopCommand", "StopMove"}, 
            {"FuelCommand", "FuelContinious"},
            {"RotateCommand", "RotateContinious"}};

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GetStrategy", 
        (object[] args) => 
        {
            string arg = (string) args[0];
            return d[arg];
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create object by dictionary", 
        (object[] args) => 
        {
            Dictionary<string, object> dic = (Dictionary<string, object>) args[0];
            string typeCmd = (string) dic["type"];
            string strategy = Hwdtech.IoC.Resolve<string>("GetStrategy", typeCmd);

            return Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>(strategy, dic);
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "StopCommand", 
        (object[] args) => 
        {
            Mock<ICommand> command = new();
            return command.Object;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "FuelCommand", 
        (object[] args) => 
        {
            Mock<ICommand> command = new();
            return command.Object;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "RotateCommand", 
        (object[] args) => 
        {
            Mock<ICommand> command = new();
            return command.Object;
        }).Execute();

    }
    [Fact]
    public void ContractTest()
    {
        var scope = IoCInit();

        Contract contract = new();
        JsonDictionary jsonDictionary = new();
        JsonDictionary jsonDictionary1 = new();

        IEnumerable<KeyValuePair<string, object>> entries = jsonDictionary.Entries;

        Dictionary<string, object> dict = new(){{"type", "MoveCommand"}, {"ID", "356"}, {"gameID", "12547"}, {"thread", "7"}};
        FormatterConverter formatterConverter = new();
        SerializationInfo serializationInfo = new(typeof(JsonDictionary), formatterConverter);
        StreamingContext streamingContext = new();

        serializationInfo.AddValue("MoveCommand", "CreateMoveCommandContinious");

        JsonDictionary jsonDictionary2 = new(dict);

        contract.json = jsonDictionary;

        MemoryStream memoryStream = new();

        try
        {
            jsonDictionary2.GetObjectData(serializationInfo, streamingContext);
            JsonDictionary jsonDictionary3 = new(serializationInfo, streamingContext);

            var serialize = JsonSerializer.Serialize(contract);
            var json = JsonSerializer.Deserialize<Contract>(serialize);

            var serialize1 = JsonSerializer.Serialize(jsonDictionary);
            var json1 = JsonSerializer.Deserialize<Contract>(serialize1);

            var serialize2 = JsonSerializer.Serialize(jsonDictionary1);
            var json2 = JsonSerializer.Deserialize<Contract>(serialize2);

            Assert.IsType<Contract>(json);
            Assert.IsType<Contract>(json1);
            Assert.IsType<Contract>(json2);
        }

        catch(Exception e)
        {
            Assert.Empty(e.Message);
        }
    }

    [Fact]
    public void WebApiTest()
    {
        var scope = IoCInit();

        Dictionary<string, object> dict = new(){{"MoveCommand", "CreateMoveCommandContinious"}};

        WebApi api = new();
        Contract contract = new();
        JsonDictionary jsonDictionary = new(dict);
        contract.json = jsonDictionary;
        //api.BodyEcho(contract);
        
        //Assert.IsType<Contract>(api.BodyEcho(contract));

    }

}
