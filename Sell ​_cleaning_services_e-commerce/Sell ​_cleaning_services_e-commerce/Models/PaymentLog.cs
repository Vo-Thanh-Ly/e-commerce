using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{

    public partial class PaymentLog
    {
        [Key]
        public int LogId { get; set; }

        public int PaymentId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? LogDate { get; set; }

        [Required]
        public string LogData { get; set; }

        [ForeignKey("PaymentId")]
        [InverseProperty("PaymentLogs")]
        public virtual Payment Payment { get; set; }
    }

}
