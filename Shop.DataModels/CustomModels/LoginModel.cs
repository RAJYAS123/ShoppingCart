using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shop.DataModels.CustomModels
{
    public class LoginModel
    {
        public string UserKey { get; set; }
        public string Name { get; set; }

        [Required (ErrorMessage = "* Email Is Required")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "* Password Is Required")]
        public string Password { get; set; }
    }
}
