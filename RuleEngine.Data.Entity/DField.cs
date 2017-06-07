using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RuleEngine.Data.Entity
{
    [Table("Field")]
    public class DField
    {
        [Key]
        public Guid FieldId { get; set; }
        public string Name { get; set; }
        public Guid TypeId { get; set; }
        public virtual DType Type { get; set; }
    }
}
