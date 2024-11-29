using Hwdtech;
using SpaceBattle.Lib;
using Moq;

namespace SpaceBattle.Lib.Test;

public class TransactionsTest
{

    [Fact]
    public void GetCommitedTransaction()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        var scope = Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"));

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Dictionary<string, string> transactionManager = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TransactionManager.GetByTransactionId", (object[] args) => {
            var output = "";
            transactionManager.TryGetValue((string) args[0], out output);
            if(output != "")
            {
                return output;
            }
            else{
                return null;
            }
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CurrentTransaction.Id", (object[] args) => {
           return "1";
        }).Execute();

        transactionManager["1"] = "Commited";

        TwoPhaseObject obj = new();

        obj.set_property("Velocity", 2);

        Assert.Equal(2, obj.get_property("Velocity"));
    }

    [Fact]
    public void GetNonExistentKey()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        var scope = Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"));

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Dictionary<string, string> transactionManager = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TransactionManager.GetByTransactionId", (object[] args) => {
            var output = "";
            transactionManager.TryGetValue((string) args[0], out output);
            if(output != "")
            {
                return output;
            }
            else{
                return null;
            }
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CurrentTransaction.Id", (object[] args) => {
           return "1";
        }).Execute();

        transactionManager["1"] = "Commited";

        TwoPhaseObject obj = new();

        Assert.Throws<Exception>(() => {var a = obj.get_property("Velocity");});
    }

    [Fact]
    public void GetAbortedTransaction()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        var scope = Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"));

        var currentTransactionId = "1";

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Dictionary<string, string> transactionManager = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TransactionManager.GetByTransactionId", (object[] args) => {
            var output = "";
            transactionManager.TryGetValue((string) args[0], out output);
            if(output != "")
            {
                return output;
            }
            else{
                return null;
            }
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CurrentTransaction.Id", (object[] args) => {
           return currentTransactionId;
        }).Execute();

        transactionManager["1"] = "Commited";

        TwoPhaseObject obj = new();

        obj.set_property("Velocity", 2);

        Assert.Equal(2, obj.get_property("Velocity"));

        currentTransactionId = "2";

        transactionManager["2"] = "Aborted";

        obj.set_property("Velocity", 4);

        Assert.Equal(2, obj.get_property("Velocity"));

        Assert.Equal(2, obj.get_property("Velocity"));

    }

     [Fact]
    public void SetCommitedTransaction()
    {
         new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        var scope = Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"));

        var currentTransactionId = "1";

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Dictionary<string, string> transactionManager = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TransactionManager.GetByTransactionId", (object[] args) => {
            var output = "";
            transactionManager.TryGetValue((string) args[0], out output);
            if(output != "")
            {
                return output;
            }
            else{
                return null;
            }
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CurrentTransaction.Id", (object[] args) => {
           return currentTransactionId;
        }).Execute();

        TwoPhaseObject obj = new();

        transactionManager["1"] = "Commited";

        transactionManager["2"] = "Commited";

        obj.set_property("Velocity", 4);

        currentTransactionId = "2";

        obj.set_property("Velocity", 5);

        Assert.Equal(5, obj.get_property("Velocity"));

    }

    [Fact]
    public void SetAbortedTransaction()
    {
        new Hwdtech.Ioc.InitScopeBasedIoCImplementationCommand().Execute();

        var scope = Hwdtech.IoC.Resolve<object>("Scopes.New", Hwdtech.IoC.Resolve<object>("Scopes.Root"));

        var currentTransactionId = "1";

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Dictionary<string, string> transactionManager = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "TransactionManager.GetByTransactionId", (object[] args) => {
            var output = "";
            transactionManager.TryGetValue((string) args[0], out output);
            if(output != "")
            {
                return output;
            }
            else{
                return null;
            }
        }).Execute();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "CurrentTransaction.Id", (object[] args) => {
           return currentTransactionId;
        }).Execute();

        TwoPhaseObject obj = new();

        transactionManager["1"] = "Aborted";

        transactionManager["2"] = "Commited";

        obj.set_property("Velocity", 4);

        currentTransactionId = "2";

        obj.set_property("Velocity", 5);

        Assert.Equal(5, obj.get_property("Velocity"));
    }


}
