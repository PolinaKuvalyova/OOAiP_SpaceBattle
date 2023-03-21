namespace SpaceBattle.Lib.Test;
using SpaceBattle.Lib;
using Moq;
using VectorSpaceBattle;

public class MoveCommandTests
{
    [Fact]
    public void TestPositiveMove()
    {
        Mock<IMovable> movable = new Mock<IMovable>();
        movable.SetupProperty<Vector>(m => m.position, new Vector(12, 5));
        movable.SetupGet<Vector>(m => m.velocity).Returns(new Vector(-7, 3));

        ICommand mc = new MoveCommand(movable.Object);
        mc.Execute();

        Assert.Equal(movable.Object.position, new Vector(5, 8));
    }
    [Fact]
    public void TestNegativePositionMove()
    {
        Mock<IMovable> movable = new Mock<IMovable>();
        movable.Setup(x => x.position).Throws(new Exception());
        movable.SetupGet<Vector>(m => m.velocity).Returns(new Vector(-7, 3));

        ICommand mc = new MoveCommand(movable.Object);

        Assert.Throws<Exception>(() => mc.Execute());
    }
    [Fact]
    public void TestNegativeVelocityMove()
    {
        Mock<IMovable> movable = new Mock<IMovable>();
        movable.SetupProperty(x => x.position, new Vector(12, 5));
        movable.SetupGet(x => x.velocity).Throws<Exception>();

        ICommand mc = new MoveCommand(movable.Object);

        Assert.Throws<Exception>(() => mc.Execute());
    }
    [Fact]
    public void TestNegativeMoveCommandveMove()
    {
        Mock<IMovable> movable = new Mock<IMovable>();
        movable.SetupProperty(x => x.position, new Vector(12, 5));
        movable.Setup(x => x.velocity).Returns(new Vector(-7, 3, 3));

        ICommand mc = new MoveCommand(movable.Object);

        Assert.Throws<ArgumentException>(() => mc.Execute());
    }

}
