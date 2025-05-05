using System;

namespace Funkin.Core
{
    public interface ICloneable<out T> : ICloneable
    {
        new T Clone();
    }

    public interface IDeepCloneable<out T>
    {
        T DeepClone();
    }
}