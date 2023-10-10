using Hwdtech;
using Moq;

namespace SpaceBattle.Lib.Test;

public class AdapterGeneratorTests
{
    [Fact]
    public void MainTestMovable()
    {
        Assert.Equal( "public class IMovableAdapter : IMovable\n{\n\t public Vector position{ get;  set; }\n\t public Vector velocity{ get; }\n\t public IMovableAdapter (object obj)\n\t {\n\t \t this.position = Hwdtech.IoC.Resolve<Vector>(\"IUObject.Property.Get\", obj, \"position\"); \n\t \t this.velocity = Hwdtech.IoC.Resolve<Vector>(\"IUObject.Property.Get\", obj, \"velocity\"); \n\t } \n}", AdapterGenerator.Generate(typeof(IMovable))) ;
    }
}
