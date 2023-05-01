namespace SpaceBattle.Lib;

public class StopCommand : ICommand
{
    ServerThread thread;
    public StopCommand(ServerThread thread)
    {
        this.thread = thread;
    }

    public void Execute()
    {
        if(Thread.CurrentThread == thread.thread)
        {
            thread.Stop();
        }
        else
        {
            throw new Exception();
        }
    }
}
