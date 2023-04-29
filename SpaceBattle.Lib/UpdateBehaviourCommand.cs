namespace SpaceBattle.Lib;

public class UpdateBehaviourCommand : ICommand
{
    public ServerThread thread;
    public Action action;
    public UpdateBehaviourCommand(ServerThread thread, Action action)
    {
        this.thread = thread;
        this.action = action;
    }
    public void Execute()
    {
        thread.UpdateBehaviour(this.action);
    }
}