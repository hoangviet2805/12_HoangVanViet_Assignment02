using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _12_HoangVanViet_A02_BE.Models
{
    [Table("NewsArticle")]
    public class NewsArticle
    {
        public NewsArticle()
        {
            NewsTags = new HashSet<NewsTag>();
        }

        [Key]
        [StringLength(20)]
        public string NewsArticleID { get; set; } = null!;

        [StringLength(400)]
        public string? NewsTitle { get; set; }

        [Required]
        [StringLength(150)]
        public string Headline { get; set; } = null!;

        public DateTime? CreatedDate { get; set; }

        [StringLength(4000)]
        public string? NewsContent { get; set; }

        [StringLength(400)]
        public string? NewsSource { get; set; }

        public short? CategoryID { get; set; }

        public bool? NewsStatus { get; set; }

        public short? CreatedByID { get; set; }

        public short? UpdatedByID { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [ForeignKey("CategoryID")]
        [InverseProperty("NewsArticles")]
        public virtual Category? Category { get; set; }

        [ForeignKey("CreatedByID")]
        [InverseProperty("NewsArticles")]
        public virtual SystemAccount? CreatedBy { get; set; }

        [InverseProperty("NewsArticle")]
        [JsonIgnore]
        public virtual ICollection<NewsTag> NewsTags { get; set; }
    }
}
