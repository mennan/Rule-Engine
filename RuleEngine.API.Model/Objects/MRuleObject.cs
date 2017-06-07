using System;
using System.ComponentModel.DataAnnotations;

namespace RuleEngine.API.Model
{
    public class MRuleObject
    {
        public Guid RuleId { get; set; }
        [Required(ErrorMessage = "Rule name field is required.")]
        [MaxLength(100, ErrorMessage = "Rule name field must be length of 100.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Rule content field is required.")]
        public FilterRule Rules { get; set; }

        [Required(ErrorMessage = "Filter field is required.")]
        public string Filter { get; set; }

        public string Content { get; set; }
    }
}
