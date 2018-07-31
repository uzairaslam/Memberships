using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Memberships.Entities
{
    [Table("Subscription")]
    public class Subscription
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(255)]
        [Required]
        public string Title { get; set; }

        [MaxLength(2048)]
        public string Description { get; set; }

        [MaxLength(20)]
        public string RgistrationCode { get; set; }
    }
}