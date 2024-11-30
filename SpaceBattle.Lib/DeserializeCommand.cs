namespace SpaceBattle.Lib;
using System.Collections.Generic;

public class DesserializeSendCommand: ICommand{

    private Dictionary<string, object> _props;

    public DesserializeSendCommand(Dictionary<string, object> properties)
    {
        this._props = properties;
    }
    public void Execute()
    {   
        string MessageType = (string) _props["type"];

        SpaceBattle.Lib.ICommand cmd = Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Create " + MessageType + " by Message", _props);

        Hwdtech.IoC.Resolve<object>("Send Command", cmd);
    }
}