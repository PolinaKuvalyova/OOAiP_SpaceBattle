using Hwdtech;
using VectorSpaceBattle;
using Moq;
using System.Collections.Concurrent;
using SpaceBattle.Lib;

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
    
}
