namespace SpaceBattle.Lib;
public class ServerThread
{
    private Thread thread;
    private bool stop = false;
    private Action strategy;
    IReceiver receiver;
    public ServerThread(IReceiver receiver)
    {
        this.receiver = receiver;

        this.strategy = () => {
            HandleCommand();
        };

        this.thread = new Thread(() => 
        {
            while(!stop) 
            {
                strategy();
            }
        });
    }
    internal void HandleCommand()
    {
        var cmd = receiver.Receive();
        cmd.Execute();
    }

    void UpdateBehaviour(Action newBeh)
    {
        strategy = newBeh;
    }

    void Stop()
    {
        stop = true;
    }
    public void Execute(){
        thread.Start();
    }
}