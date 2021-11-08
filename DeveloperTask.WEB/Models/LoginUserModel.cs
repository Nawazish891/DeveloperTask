using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeveloperTask.WEB.Models
{
    public class LoginUserModel
    {
        #region ---- Properties ----
        [Required(ErrorMessage = "This field is required")]
        [DisplayName("User Name")]
        public string Username { get; set; }

        [Required(ErrorMessage ="This field is required")]
        public string Password { get; set; }
        public string Email { get; set; }
        #endregion
    }
}