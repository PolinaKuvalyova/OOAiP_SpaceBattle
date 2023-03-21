using VectorSpaceBattle;

namespace SpaceBattle.Lib
{
    public interface IMovable
    {
        Vector position { set; get; }
        Vector velocity { get; }
    }

}
