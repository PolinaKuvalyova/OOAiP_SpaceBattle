using Hwdtech;
using SpaceBattle.Lib;
using Moq;
using Google.Protobuf.Collections;
namespace Spaceship.IoC.Test.No.Strategies;

public class gRPCTests
{
    [Fact]
    public void MapperTest()
    {
        Dependencies.Run();

        Dictionary<string, object> dict = new();

        MapField<string, string> map = new();

        map.Add("key", "value");

        dict = Hwdtech.IoC.Resolve<Dictionary<string, object>>("Map protobuf to dict", map);

        Assert.Equal(new Dictionary<string, object>(){{"key", "value"}}, dict);
   }
}
