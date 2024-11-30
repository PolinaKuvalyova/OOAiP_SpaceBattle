namespace SpaceBattle.Lib;

using System.Collections.Concurrent;
using System.Collections.Generic;

public class BCPushCommand : ICommand
{
    BlockingCollection<SpaceBattle.Lib.ICommand> _q;

    List<SpaceBattle.Lib.ICommand> _cmds;

    public BCPushCommand(BlockingCollection<SpaceBattle.Lib.ICommand> q, List<SpaceBattle.Lib.ICommand> cmds)
    {
        this._q = q;
        this._cmds = cmds;
    }

    public void Execute()
    {
        foreach(SpaceBattle.Lib.ICommand cmd in this._cmds)
        {
            _q.Add(cmd);
        }
    }
}
