using Hwdtech;
using Moq;
using System.Collections.Concurrent;

namespace SpaceBattle.Lib.Test;

public class ServerThreadTest
{
    [Fact]
    public object IoCInit()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

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
                Action ac = (Action) args[1];
                ActionCommand action = new(ac);
                queue.Add(action);
            }

            sender.Setup(s => s.Send(It.IsAny<ICommand>())).Callback<ICommand>((command => queue.Add(command)));

            ServerThread thread = new ServerThread(receiver.Object);
            thread.Execute();

            int id = (int) args[0];
            dictionarySend.Add(id, sender.Object);
            dictionaryThread.Add(id, thread);
            return thread;

        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Command", 
        (object[] args) => 
        {
            int id = (int) args[0];
            ICommand cmd = (ICommand) args[1];
            ISender send = dictionarySend[id];

            
            return new ActionCommand(() => {send.Send(cmd);});
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Hard Stop The Thread", 
        (object[] args) => 
        {
            int id = (int) args[0];
            Action action = () => {};
            ActionCommand sendHardStop = new ActionCommand(() => {});
            if(args.Count() == 2)
            {
                Action ac = (Action) args[1];

                ServerThread thread_ = Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", id);
                UpdateBehaviourCommand updateBeh= new(thread_, thread_.strategy + ac);
                action = () => {
                    Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", id, updateBeh).Execute();};
            }
            HardStop hardStop = new(dictionaryThread[id]);

            sendHardStop = new ActionCommand(action + (() => {
                    Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", id, hardStop).Execute();
                }));
            return sendHardStop;
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Soft Stop The Thread", 
        (object[] args) => 
        {
            int id = (int) args[0];
            ISender sender = dictionarySend[id];
            ActionCommand sendSoftStop = new ActionCommand(() => {});

            if(args.Count() == 2)
            {
                Action ac = (Action) args[1];
                ActionCommand action = new(ac);
                SoftStop softStop = new(dictionaryThread[id], ac);
                sendSoftStop = new ActionCommand(() => {
                    Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", id, softStop).Execute();
                });
            }
            else{
                SoftStop softStop = new(dictionaryThread[id]);
                sendSoftStop = new ActionCommand(() => {
                    Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", id, softStop).Execute();
                });
            }
            return (SpaceBattle.Lib.ICommand)sendSoftStop;
        }).Execute();
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get Thread by id", 
        (object[] args) => 
        {
            int id = (int) args[0];
            return dictionaryThread[id];
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get Send by id", 
        (object[] args) => 
        {
            int id = (int) args[0];
            return dictionarySend[id];
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get id by thread", 
        (object[] args) => 
        {
            ServerThread ac = (ServerThread) args[0];
            var id = dictionaryThread.FirstOrDefault(x => x.Value == ac).Key;
            
            return (object) id;
        }
        ).Execute();

        return scope;
    }

    [Fact]
    public void SoftStopThreadTest()
    {
        var scope = IoCInit();

        AutoResetEvent event_ = new AutoResetEvent(false);

        Hwdtech.IoC.Resolve<object>("Create And Start Thread", 4, () => {IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();});

        Hwdtech.IoC.Resolve<object>("Create And Start Thread", 16, () => {IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();});
        ISender sender = Hwdtech.IoC.Resolve<ISender>("Get Send by id", 16);

        BlockingCollection<ICommand> queue = new BlockingCollection<ICommand>();
        
        IReceiver receiver = (Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", 4)).receiver;
        ServerThread st = Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", 4);
        ServerThread st1 = Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", 16);

        var cmd = new ActionCommand(
            () => {
                Assert.False(receiver.IsEmpty());
            }
        );
        var cmd1 = new ActionCommand(
            () => {}
        );
        var cmd2 = new ActionCommand(
            () => {
                event_.Set();
            }
        );
        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Soft Stop The Thread", 4, () => {event_.Set();}).Execute();
        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Soft Stop The Thread", 16).Execute();
        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", 4, cmd).Execute();
        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", 4, cmd1).Execute();
        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", 4, cmd2).Execute();

        event_.WaitOne();

        Assert.True(receiver.IsEmpty());
        event_.WaitOne();

        Assert.True(st.stop);
        st1.Stop();
    }
    [Fact]
    public void HardStopThreadTest()
    {
        var scope = IoCInit();

        AutoResetEvent event_ = new AutoResetEvent(false);

        Hwdtech.IoC.Resolve<object>("Create And Start Thread", 4, () => {IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();});
        Hwdtech.IoC.Resolve<object>("Create And Start Thread", 14, () => {IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();});

        BlockingCollection<ICommand> queue = new BlockingCollection<ICommand>();
        
        IReceiver receiver =( Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", 4)).receiver;
        ServerThread st = Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", 4);
        ServerThread st1 = Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", 14);


        var cmd = new ActionCommand(
            () => {
                Assert.False(receiver.IsEmpty());
            }
        );
        var cmd1 = new ActionCommand(
            () => {}
        );

        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", 4, cmd).Execute();
        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Hard Stop The Thread", 4, () => {event_.Set();}).Execute();
        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", 4, cmd1).Execute();

        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", 14, cmd1).Execute();

        event_.WaitOne();

        Assert.False(receiver.IsEmpty());

        Assert.True(st.stop);
        st1.Stop();
    }

    [Fact]
    public void StopThreadTestException()
    {
        var scope = IoCInit();

        Hwdtech.IoC.Resolve<object>("Create And Start Thread", 40, () => {IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();});
        ServerThread st = Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", 40);

        Hwdtech.IoC.Resolve<object>("Create And Start Thread", 7, () => {IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();});
        IReceiver receiver =( Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", 7)).receiver;
        ServerThread st_stop = new(receiver);
        StopCommand stopCommand = new(st_stop);
        st_stop.Execute();

        var cmd = new ActionCommand(
            () => {
                Assert.Throws<Exception>(() => {stopCommand.Execute();});
            }
        );

        Hwdtech.IoC.Resolve<SpaceBattle.Lib.ICommand>("Send Command", 40, cmd).Execute();
        st.Stop();
        st_stop.Stop();
    }

    [Fact]
    public void SoftStopTest()
    {
        var scope = IoCInit();

        Hwdtech.IoC.Resolve<object>("Create And Start Thread", 8);
        ServerThread stSoftStop = Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", 8);

        Hwdtech.IoC.Resolve<object>("Create And Start Thread", 88);
        ServerThread stSoftStop1 = Hwdtech.IoC.Resolve<ServerThread>("Get Thread by id", 88);
        SoftStop softStop = new(stSoftStop);
        SoftStop softStop1 = new(stSoftStop1);
        
        Assert.Equal(softStop.action, softStop1.action);
        Assert.IsType<Action>(softStop.action);

        Mock<IReceiver> r = new();
        BlockingCollection<ICommand> queue = new BlockingCollection<ICommand>();

        //r.Setup(r => r.Receive()).Returns(() => queue.Take());
        //r.Setup(r => r.IsEmpty()).Returns(() => queue.Count == 0);

        ServerThread thread = new(r.Object);

        SoftStop s = new(thread);

        s.Get()();

        SoftStop s2 = new(thread);

        Assert.Equal(s2.Get(), s.Get());
        stSoftStop.Stop();
        stSoftStop1.Stop();
    }
}
