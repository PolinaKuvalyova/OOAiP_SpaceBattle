namespace SpaceBattle.Lib;

public class SoftStop : ICommand
{
    public ServerThread thread;
    public Action action {get;}
    public SoftStop(ServerThread thread)
    {
        this.action = new Action(() => {});
        this.thread = thread;
    }

    public SoftStop(ServerThread thread, Action action)
    {
        this.thread = thread;
        this.action = action;
    }

    //public Action Get()
    //{
    //    return this.action;
    //}
    public void Execute()
    {
        int id = Hwdtech.IoC.Resolve<int>("Get id by thread", thread);
        new UpdateBehaviourCommand(thread, () => {
            if(!(thread.receiver.IsEmpty())){
                thread.HandleCommand();
            }
            else{
                Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Send Command", id, Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Hard Stop The Thread", id, this.action)).Execute();
            }
        }).Execute();
    }
}
