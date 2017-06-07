using System;
using System.ComponentModel.DataAnnotations;

namespace RuleEngine.API.Model
{
    public class MField : BaseModel
    {
        public Guid FieldId { get; set; }

        [Required(ErrorMessage = "Field name is required.")]
        [MaxLength(500, ErrorMessage = "Field name must be length of 500.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Field type is required.")]
        public Guid TypeId { get; set; }
        public MType Type { get; set; }
    }
}
