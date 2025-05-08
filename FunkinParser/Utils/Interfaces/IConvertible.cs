namespace Funkin.Utils.Interfaces
{
    public interface IConvertible<out T1>
    {
        T1? Convert();
    }
    
    public interface IConvertible<T1, T2>
    {
        (T1?, T2?) Convert();
    }
    
    public interface IConvertible<T1, T2, T3>
    {
        (T1?, T2?, T3?) Convert();
    }
    
    public interface IConvertible<T1, T2, T3, T4>
    {
        (T1?, T2?, T3?, T4?) Convert();
    }
}