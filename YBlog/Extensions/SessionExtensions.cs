using Newtonsoft.Json;

namespace YBlog.Extensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T? Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(value);
        }

        public static void SetBool(this ISession session, string key, bool value)
        {
            session.SetInt32(key, value ? 1 : 0);
        }
        public static bool? GetBool(this ISession session, string key)
        {
            var value = session.GetInt32(key);
            if (value == null)
            {
                return null;
            }
            return value == 1;
        }
    }
}
