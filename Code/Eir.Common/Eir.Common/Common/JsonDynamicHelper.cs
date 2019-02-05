using System;
using System.Linq;
using Eir.Common.Logging;
using Newtonsoft.Json.Linq;

namespace Eir.Common.Common
{
    //time to get funky 
    public class JsonDynamicHelper
    {
        public Guid GetGuid(JToken funky, string[] names)
        {
            try
            {
                JToken test = funky;
                foreach (string item in names)
                {
                    test = test[item];
                }
                string ret = test.Value<string>();
                return Guid.Parse(ret);
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting Guid {names?.FirstOrDefault()}", e);
                return Guid.Empty;
            }

        }

        //private Guid GetGuid(JToken funky, string name)
        //{
        //    try
        //    {


        //        JToken test = funky[name];
        //        string ret = test.Value<string>();
        //        return Guid.Parse(ret);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Add($"Failed Getting Guid {name}", e);
        //        return Guid.Empty;
        //    }
        //}

        public int GetInt(JToken funky, string name)
        {
            try
            {
                JToken test = funky[name];

                var d = test.Value<int>();
                return d;
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting int {name}", e);
                return -1;
            }
        }

        public DateTime GetDate(JToken funky, string name)
        {
            try
            {
                JToken test = funky[name];
                DateTime ret = test.Value<DateTime>();
                return ret;
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting Date {name}", e);
                return DateTime.MinValue;
            }
        }

        public string GetString(JToken funky, string[] names)
        {
            try
            {
                JToken test = funky;
                foreach (string item in names)
                {
                    test = test[item];
                }

                string ret = test.Value<string>();
                return ret;
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting Strings {names?.FirstOrDefault()}", e);
                return string.Empty;
            }
        }

        public string GetString(JToken funky, string name)
        {
            try
            {

                JToken test = funky[name];
                string ret = test.Value<string>();
                return ret;
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting String {name}", e);
                return string.Empty;
            }
        }

        public bool GetBool(JToken funky, string name)
        {
            try
            {
                JToken test = funky[name];
                bool ret = test.Value<bool>();
                return ret;
            }
            catch (Exception e)
            {
                Log.To.Main.AddException($"Failed Getting bool {name}", e);
                return false;
            }
        }
    }
}
