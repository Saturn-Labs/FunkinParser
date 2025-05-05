namespace Funkin.Core
{
    public interface IData
    {
        object? this[string key] { get; }
        object? GetValue(string key);
        T GetValue<T>(string key);
        bool HasValue(string key);
    }
}