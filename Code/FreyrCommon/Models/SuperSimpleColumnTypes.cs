using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace FreyrCommon.Models
{
   
    public class SuperSimpleColumnTypes
    {
        public abstract class EmptySuperType { internal EmptySuperType(){} }

        [DebuggerDisplay("{ColumnOne} = {ColumnTwo}")]
        public class TwoColumnType : EmptySuperType
        {
            public string ColumnOne { get; set; }
            public string ColumnTwo { get; set; }
        }

        public static List<T>SerializeJsonInto<T>(string json) where T : EmptySuperType
        {
            
            dynamic dynJson = JsonConvert.DeserializeObject(json);
            if (typeof(T) == typeof(TwoColumnType))
            {
                var ret = new List<TwoColumnType>();
                foreach (var item in dynJson)
                {
                    ret.Add(new TwoColumnType
                    {
                        ColumnOne = item.Name,
                        ColumnTwo = item.Value,
                    });
                }
                
                return ret as List<T>;
            }
            throw new NotSupportedException("This type is not serialized here.");

        }
    }
}
