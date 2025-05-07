using System.Reflection;
using Funkin.Data.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Funkin.Data.ContractResolvers
{
    public class SelectableJsonIgnoreContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            var attr = member.GetCustomAttribute<SelectableJsonIgnoreAttribute>();
            if (attr == null) 
                return prop;
            
            prop.ShouldSerialize = _ => !attr.IgnoreOnWrite;
            prop.ShouldDeserialize = _ => !attr.IgnoreOnRead;
            if (attr.IgnoreOnWrite)
                prop.NullValueHandling = NullValueHandling.Ignore;
            return prop;
        }
    }
}