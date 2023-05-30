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
    public void StrategyTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();
        Dictionary<int, Queue<SpaceBattle.Lib.ICommand>> dictionaryQueue = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get object by id", 
        (object[] args) => 
        {
            int id = (int) args[0];
            return dictionaryQueue[id];
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "MoveCommand", 
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
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Contract contract = new();
        JsonDictionary jsonDictionary = new();
        JsonDictionary jsonDictionary1 = new();

        IEnumerable<KeyValuePair<string, object>> entries = jsonDictionary.Entries;

        Dictionary<string, object> dict = new(){{"type", "move"}, {"gameId", "3"}, {"ID", "12547"}, {"action", "start"}};
        FormatterConverter formatterConverter = new();
        SerializationInfo serializationInfo = new(typeof(JsonDictionary), formatterConverter);
        StreamingContext streamingContext = new();

        serializationInfo.AddValue("thread", "3");

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
}
