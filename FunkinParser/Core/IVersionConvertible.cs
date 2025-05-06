namespace Funkin.Core
{
    public interface IVersionConvertible<out T>
    {
        T Convert();
    }
}