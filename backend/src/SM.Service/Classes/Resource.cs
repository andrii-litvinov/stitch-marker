using System.Collections.Generic;
using Newtonsoft.Json;

namespace SM.Service.Classes
{
    [JsonConverter(typeof(ResourceJsonConverter))]
    public class Resource
    {
        public Resource(object value)
        {
            Value = value;
            Links = new List<Link>();
        }

        public object Value { get; }
        public List<Link> Links { get; }
    }
}
