using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RuleEngine.Data.Entity
{
    [Table("Rule")]
    public class DRule
    {
        [Key]
        public Guid RuleId { get; set; }
        public string Name { get; set; }
        public string Filter { get; set; }
        public string Content { get; set; }
    }
}
