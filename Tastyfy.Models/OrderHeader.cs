using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Tastyfy.Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        [Required] public string UserId { get; set; }

        [ForeignKey("UserId")] 
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required] 
        public DateTime OrderDate { get; set; }


        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [DisplayName(displayName:"Order Total")]
        public double OrderTotal { get; set; }

        [Required]
        [DisplayName(displayName: "Pick Up Time")]
        public DateTime PickupTime { get; set; }

        [Required]
        [NotMapped]
        [DisplayName(displayName: "Pick Up Date")]
        public DateTime PickUpDate { get; set; }

        public string Status { get; set; }

        public string PaymentStatus { get; set; }

        public string Comments { get; set; }

        [DisplayName(displayName:"Pickup Name")]
        public string PickUpName { get; set; }


        [DisplayName(displayName: "Phone Number")]
        public string PhoneNumber { get; set; }

        public string TransactionId { get; set; }
    }
}
