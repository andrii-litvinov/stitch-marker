using System.Collections.Generic;
using Newtonsoft.Json;

namespace SM.Service.Classes
{
    [JsonConverter(typeof(ResourceJsonConverter))]
    public class Resource
    {
        public object Value { get; set; }
        public List<Links> Links { get; set; }
    }
}
