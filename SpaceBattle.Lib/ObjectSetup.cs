using Hwdtech;
using VectorSpaceBattle;
namespace SpaceBattle.Lib;

public class ObjectSetuper
{
    static int i = 0;
    public static void SetupObject(object target)
    {
        Dictionary<string, object> obj = (Dictionary<string, object>) target;
        IoC.Resolve<SpaceBattle.Lib.ICommand>("IUObject.Property.Set", obj, "Position", IoC.Resolve<Vector>(IoC.Resolve<string>("Game.Position.Type.Get"), i)).Execute();
        IoC.Resolve<SpaceBattle.Lib.ICommand>("IUObject.Property.Set", obj, "Fuel", 100).Execute();
        IoC.Resolve<SpaceBattle.Lib.ICommand>("IUObject.Property.Set", obj, "OwnerID", ((i % IoC.Resolve<int>("Game.NumberOfPlayers")) + 1).ToString()).Execute();
        IoC.Resolve<SpaceBattle.Lib.ICommand>("Game.Current.AddObject", (i + 1).ToString(), (object)obj).Execute();
        i++;
    }
}
