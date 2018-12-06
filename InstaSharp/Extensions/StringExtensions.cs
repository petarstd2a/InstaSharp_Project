using System;
using System.Text.RegularExpressions;

namespace InstaSharp.Extensions
{
    public static class StringExtensions
    {
        public static String IncludeHashtags(this String str)
        {
            if (String.IsNullOrWhiteSpace(str)) return str;

            var replaced = Regex.Replace(str, @"(?:(?<=\s)|^)#(\w*[A-Za-z_]+\w*)$", "<a href=\"#\">#$1</a>");
            return replaced;
        }

        public static String FriendlyDate(this String str)
        {
            // https://www.dotnetperls.com/pretty-date

            // Get time span elapsed since the date.
            TimeSpan s = DateTime.Now.Subtract(DateTime.Parse(str));

            // Get total number of days elapsed.
            int dayDiff = (int) s.TotalDays;

            // Get total number of seconds elapsed.
            int secDiff = (int) s.TotalSeconds;

            // Don't allow out of range values.
            if (dayDiff < 0)
            {
                return null;
            }

            // Handle same-day times.
            if (dayDiff == 0)
            {
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "just now";
                }

                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "1m";
                }

                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    return string.Format("{0}m", Math.Floor((double) secDiff / 60));
                }

                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "1h";
                }

                // Less than one day ago.
                if (secDiff < 86400)
                {
                    return string.Format("{0}h", Math.Floor((double) secDiff / 3600));
                }
            }

            // Handle previous days.
            if (dayDiff < 7)
            {
                return string.Format("{0}d", dayDiff);
            }

            // Older than a week
            return string.Format("{0}w", Math.Ceiling((double) dayDiff / 7));
        }
    }
}