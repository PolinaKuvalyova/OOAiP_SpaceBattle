using VectorSpaceBattle;

namespace SpaceBattle.Lib
{
    public class StartMoveCommand : ICommand
    {
        IUObject u_obj{ get; set; }
        Queue<ICommand> queue{ get; set; }
        Vector velocity { get; set; }
        public StartMoveCommand(IUObject u_obj){
            this.u_obj = (IUObject) u_obj.get_property("object");
            this.velocity = (Vector) u_obj.get_property("velocity");
            this.queue = Hwdtech.IoC.Resolve<Queue<ICommand>>("Game.Queue");
        }
        public void Execute()
        {
            this.u_obj.set_property("velocity", velocity);
            IMovable movable = Hwdtech.IoC.Resolve<IMovable>("Adapters.IUObject.IMovable", u_obj);
            ICommand mcd = new MoveCommand(movable);
            ICommand commands = new AddCommand(mcd, queue);
            MacroCommand macro = new MacroCommand(new List<ICommand>{mcd, commands});
            
            queue.Enqueue(macro);
        }
    }
}
