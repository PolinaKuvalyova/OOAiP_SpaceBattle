using Hwdtech;
using VectorSpaceBattle;
namespace SpaceBattle.Lib.Test;

public class GameInitCommandTests
{
    [Fact]
    public void RegisterDependency()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        var scope = Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"));

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "GameInitCommand", (object[] args) => 
        {
            return new InitCommand((int)args[0]);
        }).Execute();
         Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.NumberOfPlayers", (object[] args) => 
        {
            return (object)2;
        }).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register" ,"Game.Position.Type.Get", (object[] args) => 
        {
            return "WallByWall";
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register" ,"WallByWall", (object[] args) => 
        {
            int i = (int) args[0];
            return new Vector((i - (i%2))*5, (i%2)*5);
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "IUObject.Property.Set", (object[] args) => 
        {
            Dictionary<string, object> obj = (Dictionary<string, object>)args[0];
            string propname = (string)args[1];
            object value = (object)args[2];
            return new ActionCommand(() => 
            {
                if (obj.ContainsKey(propname))
                {
                    obj[propname] = value;
                }
                else
                {
                    obj.Add(propname, value);
                }
            });
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "IUObject.Property.Get", (object[] args) => 
        {
            Dictionary<string, object> obj = (Dictionary<string, object>)args[0];
            string propname = (string)args[1];
            if (obj.ContainsKey(propname))
            {
                return obj[propname];
            }
            else
            {
                throw new Exception();
            }
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "IUObject.Property.Delete", (object[] args) => 
        {
           Dictionary<string, object> obj = (Dictionary<string, object>)args[0];
            string propname = (string)args[1];
            return new ActionCommand(() => 
            {
                if (obj.ContainsKey(propname))
                {
                    obj.Remove(propname);
                }
            });
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Object.Setup", (object [] args) => 
        {
            return new ActionCommand(() => 
            {
                ObjectSetuper.SetupObject(args[0]);
            });
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Initialize.Objects", (object [] args) => 
        {
            List<Dictionary<string, object>> list = new();
            for(int i = 0; i < (int) args[0]; i++)
            {
                list.Add(new Dictionary<string, object>());
            }
            return list;
        }).Execute();
    }
}
