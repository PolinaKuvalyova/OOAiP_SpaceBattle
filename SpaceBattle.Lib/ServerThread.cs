using System;
using System.Threading;
using Hwdtech;
namespace SpaceBattle.Lib;

public class ServerThread
{
    public Thread thread;
    public IReceiver receiver;
    public IReceiver orderReciever;
    public bool stop = false;
    public Action strategy;

    internal void Stop() => stop = true;

    internal void HandleCommand()
    {
        if(!receiver.IsEmpty())
        {
            var cmd = receiver.Receive();

            cmd.Execute();
        }
        
        if(!orderReciever.IsEmpty())
        {
            var order = orderReciever.Receive();

            order.Execute();
        }
    }
    public ServerThread(IReceiver queue, IReceiver orderQueue)
    {
        this.orderReciever = orderQueue;
        this.receiver = queue;
        strategy = () =>
        {
            HandleCommand();
        };

        thread = new Thread(() =>
        {
            while (!stop) strategy();
        });
    }
    internal void UpdateBehaviour(Action newBehaviour)
    {
        strategy = newBehaviour;

    }
    public void Start()
    {
        thread.Start();
    }
}
