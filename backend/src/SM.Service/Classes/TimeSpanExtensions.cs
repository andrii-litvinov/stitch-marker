using System;

namespace SM.Service.Classes
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Seconds(this int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        } 
    }
}
