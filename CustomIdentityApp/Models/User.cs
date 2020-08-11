using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomIdentityApp.Models
{
    public class User : IdentityUser
    {
        public DateTime Registered { get; set; }

        public DateTime LoggedIn { get; set; }

        public bool IsBlocked { get; set; }

        [NotMapped]
        public bool Checked { get; set; }
    }
}
