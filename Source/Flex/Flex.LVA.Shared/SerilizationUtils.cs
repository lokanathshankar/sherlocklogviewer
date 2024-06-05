using System.Text.Json;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Flex.LVA.Shared
{
    public static class SerilizationUtils
    {
        internal static string SerilizeToJson(object theObject)
        {
            return JsonSerializer.Serialize(theObject);
        }

        public static void SerilizeToJsonBytes(Stream theStream, object theObject)
        {
            JsonSerializer.Serialize(theStream, theObject);
        }

        public static T DeSerilizeFromJson<T>(string theObject)
        {
            return JsonSerializer.Deserialize<T>(theObject);
        }

        internal static string SerilizeToJsonReadable(object theObject)
        {
            return JsonSerializer.Serialize(theObject, new JsonSerializerOptions() { WriteIndented = true });
        }
    }
}
