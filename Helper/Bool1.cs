namespace OnlineGallery.Helper
{
    public static class Bool1
    {
        public static void SetBool(this ISession session, string key, bool value)
        {
            session.SetString(key, value ? "1" : "0");
        }

        public static bool? GetBool(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (value == null)
                return null;
            return value == "1";
        }
    }

}
