namespace OnlineGallery.Helper
{
    public static class TimeAthens
    {
        public static DateTime GetAthensTime()
        {
            TimeZoneInfo athensTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Athens");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, athensTimeZone);
        }
    }
}
