namespace Spaceship.IoC.Test.No.Strategies;
using System.Collections.Concurrent;
using Moq;
using Hwdtech;
using SpaceBattle.Lib;
using System.Threading;

public class Stateful
{
    [Fact]
    public void WrongThreadStop()
    {
        Dependencies.Run();
        BlockingCollection<SpaceBattle.Lib.ICommand> q = new();
        BlockingCollection<SpaceBattle.Lib.ICommand> q1 = new();
        BlockingCollection<SpaceBattle.Lib.ICommand> q2 = new();
        BlockingCollection<SpaceBattle.Lib.ICommand> q3 = new();

        AutoResetEvent waiter = new(false);
        ISender sender = new SenderAdapter(q);
        IReceiver receiver = new RecieverAdapter(q);
        IReceiver receiver1 = new RecieverAdapter(q1);
        IReceiver receiver2 = new RecieverAdapter(q2);
        IReceiver receiver3 = new RecieverAdapter(q3);
        ServerThread thread = new(receiver, receiver2);
        ServerThread wrongthread = new(receiver1, receiver3);

        Action action = () => {
            Assert.Throws<Exception>(() => {
            waiter.Set();
            new HardStop(wrongthread).Execute();
            });
        };

        q.Add(new ActionCommand(action));

        thread.Start();

        waiter.WaitOne();
    }
    
    [Fact]
    public void RecieverAdapterTests()
    {
        BlockingCollection<SpaceBattle.Lib.ICommand> q = new();

        Mock<SpaceBattle.Lib.ICommand> cmd = new();

        q.Add(cmd.Object);

        IReceiver rec = new RecieverAdapter(q);

        Assert.Equal(cmd.Object, rec.Receive());

        Assert.True(rec.IsEmpty());
    }

    [Fact]
    public void SenderAdapterTests()
    {
        BlockingCollection<SpaceBattle.Lib.ICommand> q = new();

        Mock<SpaceBattle.Lib.ICommand> cmd = new();

        ISender rec = new SenderAdapter(q);

        Assert.Empty(q);

        rec.Send(cmd.Object);

        Assert.Single(q);

    }

    [Fact]
    public void AdaptersFieldsTest()
    {
        BlockingCollection<SpaceBattle.Lib.ICommand> q = new();

        RecieverAdapter rec = new RecieverAdapter(q);

        SenderAdapter snd = new SenderAdapter(q);

        Assert.Equal(q, snd.queue);

        Assert.Equal(q, rec.queue);
    }

    [Fact]
    public void SendSingleCommandIntoLambdaInitializedThread()
    {
        Dependencies.Run();

        ServerThread thread = IoC.Resolve<ServerThread>("Create and Start Thread", 1, () => {});

        AutoResetEvent waiter = new(false);

        ActionCommand cmd =  new(() => {Assert.Single(((RecieverAdapter)thread.receiver).queue);});

        IoC.Resolve<object>("Send Command", 1, cmd);

        cmd =  new(() => {waiter.Set();});

        IoC.Resolve<object>("Send Command", 1, cmd);

        waiter.WaitOne();

        Assert.Empty(((RecieverAdapter)thread.receiver).queue);
    }

    [Fact]
    public void SendSingleCommandIntoLambdaLessInitializedThread()
    {
        Dependencies.Run();

        ServerThread thread = IoC.Resolve<ServerThread>("Create and Start Thread", 1);

        AutoResetEvent waiter = new(false);

        ActionCommand cmd =  new(() => {Assert.Single(((RecieverAdapter)thread.receiver).queue);});

        IoC.Resolve<object>("Send Command", 1, cmd);

        cmd =  new(() => {waiter.Set();});

        IoC.Resolve<object>("Send Command", 1, cmd);

        waiter.WaitOne();

        Assert.Empty(((RecieverAdapter)thread.receiver).queue);
    }
    
    [Fact]
    public void SoftStopThread()
    {
        object scope = Dependencies.Run();


        AutoResetEvent waiter = new(false);

        ActionCommand cmd = new(() => {});

        ServerThread thread = IoC.Resolve<ServerThread>("Create and Start Thread", 1, () => {IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();});
        ServerThread thread2 = IoC.Resolve<ServerThread>("Create and Start Thread", 3, () => {IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();});

        IoC.Resolve<SpaceBattle.Lib.ICommand>("Soft Stop Thread", 3).Execute();
        IoC.Resolve<SpaceBattle.Lib.ICommand>("Soft Stop Thread", 1, () => {waiter.Set();}).Execute();        

        Assert.False(thread.stop);

        waiter.WaitOne();

        Assert.True(thread.stop);
    }
    

    [Fact]
    public void SoftStopAction()
    {
        BlockingCollection<SpaceBattle.Lib.ICommand> q = new();

        IReceiver ra = new RecieverAdapter(q);

        BlockingCollection<SpaceBattle.Lib.ICommand> q1 = new();

        IReceiver ra1 = new RecieverAdapter(q1);

        ServerThread thread = new(ra, ra1);

        SoftStop ssc = new(thread);

        SoftStop ssc2 = new(thread);

        Assert.Equal(ssc2.Get(), ssc.Get());
    }
    [Fact]
    public void HardStopThread()
    {
        Dependencies.Run();

        AutoResetEvent waiter = new(false);

        ServerThread thread = IoC.Resolve<ServerThread>("Create and Start Thread", 1);
        ServerThread thread2 = IoC.Resolve<ServerThread>("Create and Start Thread", 3);


        Assert.False(thread.stop);
        Assert.False(thread2.stop);

        IoC.Resolve<SpaceBattle.Lib.ICommand>("Hard Stop Thread", 1).Execute();
        IoC.Resolve<SpaceBattle.Lib.ICommand>("Hard Stop Thread", 3, () => {waiter.Set();}).Execute();

        waiter.WaitOne();

        Assert.True(thread.stop);
        Assert.True(thread2.stop);
    }
    
    [Fact]
    public void SoftAwaitTest()
    {
        var scope = Dependencies.Run();

        AutoResetEvent waiter = new(false);

        ServerThread thread = IoC.Resolve<ServerThread>("Create and Start Thread", 1, () => {IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();});

        ActionCommand cmd = new(() => {});

        Assert.True(thread.receiver.IsEmpty());

        IoC.Resolve<SpaceBattle.Lib.ICommand>("Soft Stop Thread", 1, () => {waiter.Set();}).Execute();

        IoC.Resolve<object>("Send Command", 1, cmd);

        IoC.Resolve<object>("Send Command", 1, cmd);

        waiter.WaitOne();

        Assert.True(thread.receiver.IsEmpty());
    }
    
    [Fact]
    public void HardNonAwaitTest()
    {
        var scope = Dependencies.Run();

        AutoResetEvent waiter = new(false);

        ServerThread thread = IoC.Resolve<ServerThread>("Create and Start Thread", 1, () => {IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();});

        ActionCommand cmd = new(() => {});

        Assert.True(thread.receiver.IsEmpty());

        IoC.Resolve<SpaceBattle.Lib.ICommand>("Hard Stop Thread", 1, () => {waiter.Set();}).Execute();

        IoC.Resolve<object>("Send Command", 1, cmd);

        IoC.Resolve<object>("Send Command", 1, cmd);

        waiter.WaitOne();

        Assert.False(thread.receiver.IsEmpty());
    }

    [Fact]
    public void HandleOrderTest()
    {
        AutoResetEvent waiter = new(false);
        bool isHandled = false;
        BlockingCollection<SpaceBattle.Lib.ICommand> q = new();
        BlockingCollection<SpaceBattle.Lib.ICommand> q2 = new();
        ISender sender = new SenderAdapter(q);
        IReceiver receiver = new RecieverAdapter(q);
        IReceiver receiver2 = new RecieverAdapter(q2);
        ServerThread thread = new(receiver, receiver2);

        q2.Add(new ActionCommand(() => {
            isHandled = true;
            waiter.Set();
            }));

        thread.Start();

        waiter.WaitOne();

        Assert.True(isHandled);
    }
}
