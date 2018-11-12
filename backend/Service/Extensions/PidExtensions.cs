using System.Threading.Tasks;
using Proto;

namespace Service
{
    public static class PidExtensions
    {
        public static async Task<TResponse> Request<TResponse>(this ISenderContext context, PID pid, object message) =>
            await context.RequestAsync<TResponse>(pid, message, 10.Seconds());
    }
}
