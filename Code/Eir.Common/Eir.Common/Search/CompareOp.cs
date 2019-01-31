using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Eir.Common.Search
{
    /// <summary>
    /// This is used for the generic wrapper search functionalty in the rest api to the db.
    /// <para>
    /// When adding new functionality to the search.
    /// Remember to document this in the exact same 
    /// manner as the other values.
    /// </para>
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CompareOp
    {
        [Description("")]
        None,

        /// <summary>
        /// <para>=</para>Equals
        /// </summary>
        [Description("==")]
        Equals,

        /// <summary>
        /// <para>!=</para>Not equals
        /// </summary>
        [Description("!=")]
        NotEquals,

        /// <summary>
        /// <para>&gt;=</para> Greater than or equal
        /// </summary>
        [Description(">=")]
        GreaterThanOrEqual,

        /// <summary>
        /// <para>&lt;=</para> Less than or equal
        /// </summary>
        [Description("<=")]
        LessThanOrEqual,

        /// <summary>
        /// <para>&gt;</para>Greater than
        /// </summary>
        [Description(">")]
        GreaterThan,

        /// <summary>
        /// <para>&lt;</para>Less than
        /// </summary>
        [Description("<")]
        LessThan,

        /// <summary>
        /// <para>StartsWith</para>String starts with, case sensitive
        /// </summary>
        [Description("StartsWith")]
        StartsWith,

        /// <summary>
        /// <para>Contains</para>String contains with, case sensitive
        /// </summary>
        [Description("Contains")]
        Contains,

       /// <summary>
        /// <para>EndsWith</para>String ends with, case sensitive
        /// </summary>
        [Description("EndsWith")]
        EndsWith,

        ///// <summary>
        ///// <para>EndsWith</para>String ends with, case insensitive
        ///// </summary>
        //[Description("EndsWith[OrdinalIgnoreCase]")]
        //EndsWithOrdinalIgnoreCase,

        /// <summary>
        /// <para>Match</para>String comparing with wildcard-matching
        /// </summary>
        [Description("Match")]
        Match
    }
}