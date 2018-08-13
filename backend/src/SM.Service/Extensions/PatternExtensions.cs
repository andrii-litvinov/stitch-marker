using System;
using System.Collections.Generic;

namespace SM.Service
{
    public static class PatternExtensions
    {
        public static List<Resource> ToResourseList(this PatternItems patternItems)
        {
            var result = new List<Resource>();
            foreach (var item in patternItems.Items)
            {
                var patternId = new Guid(item.Id);

                var preview = new {item.Id, item.Title, item.Height, item.Width};
                var resource = new Resource(preview)
                {
                    Links =
                    {
                        new Link {Rel = "self", Href = $"/api/patterns/{patternId}"},
                        new Link {Rel = "thumbnail", Href = $"/api/patterns/{patternId}/thumbnail"}
                    }
                };

                result.Add(resource);
            }

            return result;
        }
    }
}
