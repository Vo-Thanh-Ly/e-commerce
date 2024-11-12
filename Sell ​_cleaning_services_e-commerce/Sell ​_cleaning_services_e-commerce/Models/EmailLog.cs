using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    public class EmailLog
    {
        public int Id { get; set; }

        [Display(Name = "Địa chỉ Email người nhận")]
        [Required]
        [MaxLength(255)]
        public string ToEmail { get; set; }

        [Display(Name = "Tiêu đề Email")]
        [Required]
        [MaxLength(255)]
        public string Subject { get; set; }

        [Display(Name = "Nội dung Email")]
        public string Message { get; set; }

        [Display(Name = "Ngày gửi")]
        public DateTime SentDate { get; set; }

  

        [Display(Name = "ID người gửi")]
        public string SenderId { get; set; }

        // Thêm navigation property (optional)
        public virtual User Sender { get; set; }
    }
}
