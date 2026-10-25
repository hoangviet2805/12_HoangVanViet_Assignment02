using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _12_HoangVanViet_A02_BE.Models
{
    [Table("NewsTag")]
    public class NewsTag
    {
        [Key, Column(Order = 0)]
        [StringLength(20)]
        public string NewsArticleID { get; set; } = null!;

        [Key, Column(Order = 1)]
        public int TagID { get; set; }

        [ForeignKey("NewsArticleID")]
        [InverseProperty("NewsTags")]
        public virtual NewsArticle? NewsArticle { get; set; }

        [ForeignKey("TagID")]
        [InverseProperty("NewsTags")]
        public virtual Tag? Tag { get; set; }
    }
}
