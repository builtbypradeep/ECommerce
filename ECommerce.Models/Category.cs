using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [StringLength (30, ErrorMessage ="Value is too long.")]
        [Required]
        [DisplayName("Display Name")]
        public string Name { get; set; }

        [Range (1, 100)]
        [DisplayName("Index")]
        public int DisplayOrder { get; set; }
    }
}
