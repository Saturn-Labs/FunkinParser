namespace Funkin.Utils.Interfaces
{
    public interface IConvertible<T1>
    {
        bool TryConvert(out T1? result);
    }
    
    public interface IConvertible<T1, T2>
    {
        bool TryConvert(out T1? result1, out T2? result2);
    }
    
    public interface IConvertible<T1, T2, T3>
    {
        bool TryConvert(out T1? result1, out T2? result2, out T3? result3);
    }
    
    public interface IConvertible<T1, T2, T3, T4>
    {
        bool TryConvert(out T1? result1, out T2? result2, out T3? result3, out T4? result4);
    }
}