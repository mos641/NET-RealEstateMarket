using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RealEstateMarket.Models
{
    public class Advertisement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdId { get; set; }

        [Required]
        [RegularExpression(@"^.+\..{3}$")]
        public string FileName { get; set; }

        [Required]
        [Url]
        public string Url { get; set; }

        [Required]
        public Brokerage Brokerage { get; set; }
    }
}
