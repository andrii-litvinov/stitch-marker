using System.Collections.Generic;
using Newtonsoft.Json;

namespace Service
{
    [JsonConverter(typeof(ResourceJsonConverter))]
    public abstract class Resource
    {
        public List<Link> Links { get; } = new List<Link>();
        public abstract object GetValue();
    }

    public class Resource<T> : Resource
    {
        public Resource(T value) => Value = value;

        public T Value { get; }

        public override object GetValue() => Value;
    }
}
