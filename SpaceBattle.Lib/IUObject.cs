namespace SpaceBattle.Lib
{
    public interface IUObject
    {
        object get_property(string key);
        void set_property(string key, object value);   
    }
}
