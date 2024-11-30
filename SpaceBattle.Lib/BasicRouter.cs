namespace SpaceBattle.Lib;
using System.Collections.Generic;
public class DictRouter : IRouter{

    private Dictionary<string, ISender> routeDict;

    public DictRouter(Dictionary<string, ISender> dict){
        this.routeDict = dict;
    }

    public bool Route(string id, Dictionary<string, object> payload){
        try{
            routeDict[id].Send((SpaceBattle.Lib.ICommand) new DesserializeSendCommand(payload));
            return true;
        }
        catch{
            return false;
        }
    }
}