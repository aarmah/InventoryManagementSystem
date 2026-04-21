using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace InventoryAPI.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Description { get; set; } = string.Empty ;

        [JsonIgnore]
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
