using System;

namespace Funkin.Utils.Interfaces
{
    public interface ICloneable<out T> : ICloneable
    {
        T CloneTyped();
    }
}