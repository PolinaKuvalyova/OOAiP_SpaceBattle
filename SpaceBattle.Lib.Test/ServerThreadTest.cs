using Hwdtech;
using Moq;
using VectorSpaceBattle;
using System.Collections.Concurrent;

namespace SpaceBattle.Lib.Test;

public class ServerThreadTest
{
    [Fact]
    public void IoCInit()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scope.New", IoC.Resolve<object>("Scope.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scope.Current.Set", scope).Execute();

        IoC.Resolve<ICommand>(
        "Create And Start Thread", 
        "уникальный id потока"
        );

        IoC.Resolve<ICommand>(
        "Send Command", 
        "уникальный id потока"
        );
        IoC.Resolve<ICommand>(
        "Hard Stop The Thread", 
        "уникальный id потока"
        );
        IoC.Resolve<ICommand>(
        "Soft Stop The Thread",
        "уникальный id потока"
        );

        Dictionary<int, ISender> dictionarySend = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create And Start Thread", 
        (object[] args) => 
        {

        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Command", 
        (object[] args) => 
        {
            int id = (int) args[0];
            ICommand cmd = (ICommand) args[1];
            ISender send = dictionarySend[id];

            send.Send(cmd);
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Hard Stop The Thread", 
        (object[] args) => 
        {
            
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Soft Stop The Thread", 
        (object[] args) => 
        {
            
        }).Execute();
    }




    [Fact]
    public void ThreadServer_queue()
    {
        Mock<IReceiver> receiver= new();

        BlockingCollection<ICommand> queue = new BlockingCollection<ICommand>();

        receiver.Setup(r => r.Receive()).Returns(() => queue.Take());
        receiver.Setup(r => r.IsEmpty()).Returns(() => queue.Count == 0);

        Mock<ISender> sender = new();
        sender.Setup(s => s.Send(It.IsAny<ICommand>())).Callback<ICommand>((command => queue.Add(command)));

        //sender.Object.Send(new ActionCommand(
        //    (args) => {
        //        Thread.Sleep(1);
        //    }
        //));

        ServerThread st = new ServerThread(receiver.Object);
        st.Execute();

        //mre.WaitOne();
        //barrier.sSignalAndWait();

        Assert.Equal(0, queue.Count);
        Assert.True(receiver.Object.IsEmpty());
    }

}
