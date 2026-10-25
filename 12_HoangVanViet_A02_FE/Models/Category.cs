using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _12_HoangVanViet_A02_FE.Models
{
    [Table("Category")]
    public class Category
    {
        public Category()
        {
            InverseParentCategory = new HashSet<Category>();
            NewsArticles = new HashSet<NewsArticle>();
        }

        [Key]
        public short CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = null!;

        [Required]
        [StringLength(250)]
        public string CategoryDesciption { get; set; } = null!;

        public short? ParentCategoryID { get; set; }

        public bool? IsActive { get; set; }

        [ForeignKey("ParentCategoryID")]
        [InverseProperty("InverseParentCategory")]
        public virtual Category? ParentCategory { get; set; }

        [InverseProperty("ParentCategory")]
        [JsonIgnore]
        public virtual ICollection<Category> InverseParentCategory { get; set; }

        [InverseProperty("Category")]
        [JsonIgnore]
        public virtual ICollection<NewsArticle> NewsArticles { get; set; }
    }
}
