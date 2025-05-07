using System;

namespace Funkin.Utils.Interfaces
{
    public interface IDeepCloneable<out T> : ICloneable<T>
    {
        T DeepCloneTyped();
    }
}