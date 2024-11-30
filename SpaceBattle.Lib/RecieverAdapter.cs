using System;
using System.Collections.Concurrent;


namespace SpaceBattle.Lib;


public class RecieverAdapter : IReceiver
{
    public BlockingCollection<SpaceBattle.Lib.ICommand> queue;

    public RecieverAdapter(BlockingCollection<SpaceBattle.Lib.ICommand> queue) => this.queue = queue;

    public SpaceBattle.Lib.ICommand Receive()
    {
        return queue.Take();
    }

    public bool IsEmpty()
    {
        return (queue.Count == 0);
    }
}
