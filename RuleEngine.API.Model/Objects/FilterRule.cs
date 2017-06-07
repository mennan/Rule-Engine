using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RuleEngine.API.Model
{
    /// <summary>
    /// This class is used to define a hierarchical filter for a given collection.
    /// </summary>
    public class FilterRule
    {
        /// <summary>
        /// Condition - acceptable values are "and" and "or".
        /// </summary>
        /// <value>
        /// The condition.
        /// </value>
        [JsonProperty(PropertyName = "condition")]
        public string Condition { get; set; }
        /// <summary>
        /// The name of the field that the filter applies to.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        [JsonProperty(PropertyName = "field")]
        public string Field { get; set; }
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        /// <value>
        /// The input.
        /// </value>
        [JsonProperty(PropertyName = "input")]
        public string Input { get; set; }
        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        [JsonProperty(PropertyName = "operator")]
        public string Operator { get; set; }
        /// <summary>
        /// Gets or sets nested filter rules.
        /// </summary>
        /// <value>
        /// The rules.
        /// </value>
        [JsonProperty(PropertyName = "rules")]
        public List<FilterRule> Rules { get; set; }
        /// <summary>
        /// Gets or sets the type. Supported values are "integer", "double", "string", "date", "datetime", and "boolean".
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the value of the filter.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }
    }
}
