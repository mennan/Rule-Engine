using System;
using System.ComponentModel.DataAnnotations;

namespace RuleEngine.API.Model
{
    public class MType : BaseModel
    {
        public Guid TypeId { get; set; }

        [Required(ErrorMessage = "Type name field is required.")]
        [MaxLength(30, ErrorMessage = "Type name field must be length of 30.")]
        public string Name { get; set; }
    }
}
