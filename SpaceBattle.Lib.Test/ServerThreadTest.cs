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
        Dictionary<int, ServerThread> dictionaryThread = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Create And Start Thread", 
        (object[] args) => 
        {
            Mock<IReceiver> receiver= new();
            Mock<ISender> sender = new();
            BlockingCollection<ICommand> queue = new BlockingCollection<ICommand>();

            receiver.Setup(r => r.Receive()).Returns(() => queue.Take());
            receiver.Setup(r => r.IsEmpty()).Returns(() => queue.Count == 0);

            if(args.Count() == 2)
            {
                Action<object[]> ac = (Action<object[]>) args[1];
                ActionCommand action = new(ac, args);
                queue.Add(action);
            }

            sender.Setup(s => s.Send(It.IsAny<ICommand>())).Callback<ICommand>((command => queue.Add(command)));

            ServerThread thread = new ServerThread(receiver.Object);
            thread.Execute();

            int id = (int) args[0];
            dictionarySend.Add(id, sender.Object);
            dictionaryThread.Add(id, thread);

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
            int id = (int) args[0];
            if(args.Count() == 2)
            {
                Action<object[]> ac = (Action<object[]>) args[1];
                ActionCommand action = new(ac, args);
                Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Send Command", id, action);
            }
            HardStop hardStop = new(dictionaryThread[id]);
            Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Send Command", id, hardStop);
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Soft Stop The Thread", 
        (object[] args) => 
        {
            
        }).Execute();

        //Assert.Equal(0, queue.Count);
        //Assert.True(receiver.Object.IsEmpty());
    }

}
