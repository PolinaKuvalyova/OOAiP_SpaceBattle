namespace SpaceBattle.Lib;

public interface ISender
{
    public void Send(ICommand cmd);
}

//interface ISender{
//    void Send(object message);
//}