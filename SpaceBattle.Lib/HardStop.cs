namespace SpaceBattle.Lib;

public class HardStop : ICommand
{
    public ServerThread thread;
    public HardStop(ServerThread thread)
    {
        this.thread = thread;
    }
    public void Execute()
    {
        StopCommand command = new(thread);
        command.Execute();
    }
}