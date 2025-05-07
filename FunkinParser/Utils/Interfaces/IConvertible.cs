namespace Funkin.Utils.Interfaces
{
    public interface IConvertible<out TTarget>
    {
        TTarget Convert();
    }
}