using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RuleEngine.Data.Entity
{
    [Table("Type")]
    public class DType
    {
        [Key]
        public Guid TypeId { get; set; }
        public string Name { get; set; }
    }
}
