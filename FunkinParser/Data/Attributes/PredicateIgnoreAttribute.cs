using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Funkin.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PredicateIgnoreAttribute : Attribute
    {
        
    }
}