using Hwdtech;
using Moq;
using VectorSpaceBattle;
using System.Collections.Concurrent;

namespace SpaceBattle.Lib.Test;

public class ServerThreadTest
{
    [Fact]
    public void ThreadServer_queue()
    {
        var receiver = new Mock<IReceiver>();

        BlockingCollection<ICommand> queue = new BlockingCollection<ICommand>();

        receiver.Setup(r => r.Receive()).Returns(() => queue.Take());
        receiver.Setup(r => r.IsEmpty()).Returns(() => queue.Count == 0);

        var sender = new Mock<ISender>();
        //sender.Setup(s => s.Send(It.IsAny<ICommand>())).Callback<ICommand>((command => ));

        sender.Object.Send(new ActionCommand(
            (args) => {
                Thread.Sleep(1);
            }
        ));

        ServerThread st = new ServerThread(receiver.Object);
        st.Execute();

        //mre.WaitOne();

        //Assert.Equal(0, queue);
        //Assert.True(receiver);
    }

}