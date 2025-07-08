namespace SharpBotTopOnline
{
    public static class Tools
    {
        public static string GetPeriod() // 2021-09-01T00:00:00Z:2021-09-30T23:59:59Z
        {
            DateTime now = DateTime.UtcNow;
            DateTime firstDayOfMonth = new (now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            string startDateStr = firstDayOfMonth.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string endDateStr = now.ToString("yyyy-MM-ddTHH:mm:ssZ");
            return $"{startDateStr}:{endDateStr}";
        }

        public static string GetPreviousMonthPeriod() // 2021-08-01T00:00:00Z:2021-08-31T23:59:59Z
        {
            var now = DateTime.UtcNow;
            var firstDayOfCurrentMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            var lastDayOfPreviousMonth = firstDayOfCurrentMonth.AddDays(-1);
            var firstDayOfPreviousMonth = new DateTime(lastDayOfPreviousMonth.Year, lastDayOfPreviousMonth.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            string startDateStr = firstDayOfPreviousMonth.ToString("yyyy-MM-ddTHH:mm:ssZ");
            string endDateStr = lastDayOfPreviousMonth.ToString("yyyy-MM-ddTHH:mm:ssZ");
            return $"{startDateStr}:{endDateStr}";
        }

        public static string FormatTime(int seconds) // 1d 2h 3m 4s
        {
            int days = seconds / (24 * 3600);
            int hours = (seconds % (24 * 3600)) / 3600;
            int minutes = (seconds % 3600) / 60;
            int sec = seconds % 60;
            return $"{days}d {hours}h {minutes}m {sec}s";
        }
    }
}