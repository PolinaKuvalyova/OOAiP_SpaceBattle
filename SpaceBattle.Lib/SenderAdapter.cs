using System.Collections.Concurrent;
using System.Collections.Generic;
using Hwdtech;
namespace SpaceBattle.Lib;

public class SenderAdapter : ISender
{
    public BlockingCollection<SpaceBattle.Lib.ICommand> queue;

    public SenderAdapter(BlockingCollection<SpaceBattle.Lib.ICommand> queue) => this.queue = queue;

    public void Send(ICommand cmd)
    {        
        BCPushCommand pusher = new(queue, new List<SpaceBattle.Lib.ICommand>(){cmd});

        pusher.Execute();
    }
}
