using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    [Table("RoleClaim")]
    [Index("RoleId", Name = "IX_RoleClaim_RoleId")]
    public partial class RoleClaim
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string RoleId { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }

        [ForeignKey("RoleId")]
        [InverseProperty("RoleClaims")]
        public virtual Role Role { get; set; }
    }

}
