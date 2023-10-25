using Hwdtech;
using System.Collections.Generic;
using System;
namespace SpaceBattle.Lib;

public class InitCommand : SpaceBattle.Lib.ICommand
{
    private int objamount;
    public InitCommand(int objects)
    {
        this.objamount = objects;
    }
    public void Execute()
    {
        Dictionary<string, object> GameObjects = new();

        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Current.AddObject", (object[] args) => 
        {
            string id = (string)args[0];
            object obj = (object)args[1];
            return new ActionCommand(() => {
                GameObjects.Add(id, obj);
            });
        }).Execute();
        Console.WriteLine("initing");
        IEnumerable<object> objects = Hwdtech.IoC.Resolve<IEnumerable<object>>("Game.Initialize.Objects", this.objamount);
        Console.WriteLine("setuping");
        Hwdtech.IoC.Resolve<ICommand>("Game.Objects.Setup", objects).Execute();
        Console.WriteLine("done init");
        Hwdtech.IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Current.ObjById", (object[] args) => 
        {
            return GameObjects[(string)args[0]];
        }).Execute();
    } 
}
