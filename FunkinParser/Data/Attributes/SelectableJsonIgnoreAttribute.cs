using System;

namespace Funkin.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SelectableJsonIgnoreAttribute : Attribute
    {
        public bool IgnoreOnWrite { get; set; }
        public bool IgnoreOnRead { get; set; }
    }
}