using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ContactManager.Entity.Entity
{
    public class Contact
    {
        [Key]
        public int ID { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email is required.")]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Email is not valid")]
        [DataType(DataType.EmailAddress)]
        //[EmailAddress]
        public string Email { get; set; }
        [RegularExpression("^\\d+$", ErrorMessage = "Phone number is not valid.")]
        [MaxLength(12, ErrorMessage = "Phone number is not valid.")]
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }    
}
