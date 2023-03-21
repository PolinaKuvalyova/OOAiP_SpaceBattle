using VectorSpaceBattle;

namespace SpaceBattle.Lib
{
    public interface IMoveCommandStartable{

        IUObject u_obj{ get; set; }
        Queue<ICommand> queue{ get; set; }
        Vector velocity { get; set; }
    }
}
