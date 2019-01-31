using System.IO;
using System.Reflection;

namespace FreyrCollectorCommon.CollectorCore
{
    public class ReadEmbeddedResources
    {
        public string Read(string name)
        {
            // getting a list of resources in case its hard to guess :)
            //var auxList = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = name;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    return string.Empty;
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }
    }
}
