using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SM.Service.Extensions
{
    public static class FormFileExtensions
    {
        public static async Task<byte[]> ReadAllBytes(this IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
