using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models.ViewModel
{
    public class EmailLogToOneView
    {
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


    }
    public class EmailLogToAllView
    {
       

        [Display(Name = "Tiêu đề Email")]
        [Required]
        [MaxLength(255)]
        public string Subject { get; set; }

        [Display(Name = "Nội dung Email")]
        public string Message { get; set; }


    }
}
