using VectorSpaceBattle;

namespace SpaceBattle.Lib
{
    public class MovableAdapter : IMovable
    {
        
        public Vector position { set; get; }
        public Vector velocity { get; }
        public MovableAdapter(object[] obj){
            IUObject u_obj = (IUObject) obj[0];
            this.velocity = (Vector) u_obj.get_property("velocity");
            this.position = (Vector) u_obj.get_property("position");
        }
    }
}
