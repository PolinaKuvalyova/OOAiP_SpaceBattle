using VectorSpaceBattle;
using Moq;
namespace SpaceBattle.Lib.Test;

public class StartMoveCommandTest
{
    [Fact]
    public void MoveCommandStartableTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapters.IUObject.IMovable", 
        (object[] args) => 
        {
            MovableAdapter adapter = new MovableAdapter(args);
            return (object) 1;
        }).Execute();

        Queue<ICommand> queue = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue",
        (object[] args) =>
        {
            return queue;
        }
        ).Execute();

        Mock<IUObject> command = new();
        Mock<IUObject> obj = new();

        obj.Setup(x => x.get_property("velocity")).Returns((object) new Vector(1, 1));
        obj.Setup(x => x.get_property("position")).Returns((object) new Vector(0, 0));

        command.Setup(x => x.get_property("object")).Returns((object) obj.Object);
        command.Setup(x => x.get_property("queue")).Returns((object) queue);
        command.Setup(x => x.get_property("velocity")).Returns((object) new Vector(5, 5));

        StartMoveCommand start = new(command.Object);
        start.Execute();
        Assert.Single(queue);
        queue.Dequeue().Execute();
    }
    [Fact]
    public void AddCommandTest()
    {
        Mock<SpaceBattle.Lib.ICommand> command = new();
        Queue<SpaceBattle.Lib.ICommand> queue = new();
        new AddCommand(command.Object, queue).Execute();
    }
    [Fact]
    public void MovableAdapterTest()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"))).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Adapters.IUObject.IMovable", (object[] args) => 
        {
            MovableAdapter adapter = new MovableAdapter(args);
            return adapter;
        }).Execute();

        Mock<IUObject> obj = new();
        obj.Setup(x => x.get_property("position")).Returns(new Vector(8, 8));
        obj.Setup(x => x.get_property("velocity")).Returns(new Vector(7, 7));
        IMovable movable = Hwdtech.IoC.Resolve<IMovable>("Adapters.IUObject.IMovable", obj.Object);

        Assert.Equal(movable.velocity, new Vector(7, 7));
    }
}
