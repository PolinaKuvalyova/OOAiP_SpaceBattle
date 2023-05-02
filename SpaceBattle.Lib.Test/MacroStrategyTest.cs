using Hwdtech;
using Moq;
using VectorSpaceBattle;

namespace SpaceBattle.Lib.Test;

public class MacroBuilderTest
{
    [Fact]
    public void InitialTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapters.IUObject.Movable", (object[] args) => 
        {
            MovableAdapter adapter = new MovableAdapter(args);
            return adapter;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "SpaceBattle.Operation.ContiniousMovement.Get.Dependencies", (object[] args) =>
        {
            List<string> dependencies = new List<string>{"MoveCommand"};
            return dependencies;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "IoC.MoveCommand", (object[] args) =>
        {
            return (ICommand) new MoveCommand(Hwdtech.IoC.Resolve<IMovable>("Adapters.IUObject.Movable", args));
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "IoC.CreateMacro", (object[] args) =>
        {
            MacroStrategy macroStrategy = new();
            return macroStrategy.MacroStrategyCommand(args);
        }).Execute();

        Mock<IUObject> obj = new();
        Mock<IUObject> command = new();
        Queue<ICommand> queue = new();

        command.Setup(x => x.get_property("velocity")).Returns((object) new Vector(1, 1));
        command.Setup(x => x.get_property("position")).Returns((object) new Vector(0, 0));
        obj.Setup(x => x.get_property("object")).Returns((object) command.Object);
        obj.Setup(x => x.get_property("queue")).Returns((object) queue);

        queue.Enqueue(new AddCommand(IoC.Resolve<ICommand>("IoC.CreateMacro", "ContiniousMovement", obj.Object), queue));
        queue.Dequeue().Execute();
        Assert.Equal(2, queue.Count);
    }

    [Fact]
    public void GetSpeedTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapters.IUObject.Movable", (object[] args) => 
        {
            MovableAdapter adapter = new MovableAdapter(args);
            return adapter;
        }).Execute();

        Mock<IUObject> obj = new();
        obj.Setup(x => x.get_property("position")).Returns(new Vector(2));
        obj.Setup(x => x.get_property("velocity")).Returns(new Vector(1));
        IMovable movable = IoC.Resolve<IMovable>("Adapters.IUObject.Movable", obj.Object);

        Assert.Equal(movable.velocity, new Vector(1));
    }
}
