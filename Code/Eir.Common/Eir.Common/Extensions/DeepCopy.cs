using Newtonsoft.Json;

namespace Eir.Common.Extensions
{
    public static class DeepCopyExtension
    {
        /// <summary>
        /// Deep copy of object by using Json Serializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(this T source)
        {
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

    }
}
