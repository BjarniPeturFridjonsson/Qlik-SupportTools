using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Eir.Common.Search
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogicalOp
    {
        [Description("")]
        None,

        [Description("&&")]
        And,

        [Description("||")]
        Or
    }
}