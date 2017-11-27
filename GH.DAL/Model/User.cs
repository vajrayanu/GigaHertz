using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }

        [Required]
        public String Username { get; set; }

        [Required, DataType(DataType.Password)]
        public String Password { get; set; }

        public IList<Role> Roles { get; set; }
    }
}