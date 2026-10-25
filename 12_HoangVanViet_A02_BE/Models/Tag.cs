using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace _12_HoangVanViet_A02_BE.Models
{
    [Table("Tag")]
    public class Tag
    {
        public Tag()
        {
            NewsTags = new HashSet<NewsTag>();
        }

        [Key]
        public int TagID { get; set; }

        [StringLength(50)]
        public string? TagName { get; set; }

        [StringLength(400)]
        public string? Note { get; set; }

        [InverseProperty("Tag")]
        [JsonIgnore]
        public virtual ICollection<NewsTag> NewsTags { get; set; }
    }
}
