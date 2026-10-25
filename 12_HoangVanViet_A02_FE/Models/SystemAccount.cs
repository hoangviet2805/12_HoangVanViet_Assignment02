using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _12_HoangVanViet_A02_FE.Models
{
    [Table("SystemAccount")]
    public class SystemAccount
    {
        public SystemAccount()
        {
            NewsArticles = new HashSet<NewsArticle>();
        }

        [Key]
        public short AccountID { get; set; }

        [StringLength(100)]
        public string? AccountName { get; set; }

        [StringLength(70)]
        public string? AccountEmail { get; set; }

        public int? AccountRole { get; set; }

        [StringLength(70)]
        public string? AccountPassword { get; set; }

        [InverseProperty("CreatedBy")]
        [JsonIgnore]
        public virtual ICollection<NewsArticle> NewsArticles { get; set; }
    }
}
