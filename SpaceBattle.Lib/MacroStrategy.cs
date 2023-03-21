using Hwdtech;

namespace SpaceBattle.Lib;

public class MacroStrategy
{
    public ICommand MacroStrategyCommand(params object[] args)
    {
        string name = (string) args[0]+".Get.Dependencies";
        IUObject obj = (IUObject) args[1];

        var dependencies = IoC.Resolve<List<string>>("SpaceBattle.Operation." + name);
        List<ICommand> commands = new();

        foreach(string dependency in dependencies)
        {
            commands.Add(IoC.Resolve<ICommand>("IoC." + dependency, obj));
        }

        return new MacroCommand(commands);
    }
}
