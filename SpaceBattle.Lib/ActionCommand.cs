namespace SpaceBattle.Lib;

public class ActionCommand : ICommand
{
    private Action action;
    //private object[] args;
    public ActionCommand(Action action)
    {
        this.action = action;
        //this.args = args;
    }
    public void Execute()
    {
        action();
    }
}
