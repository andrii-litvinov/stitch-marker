using System;

namespace SM.Service.Classes
{
    internal static class TimeSpanExtensions
    {
        public static TimeSpan Seconds(this int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }
        
        public static TimeSpan Minutes(this int minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }
    }
}
