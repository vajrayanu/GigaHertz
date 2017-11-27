using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GH.DAL.Model
{
    public class Role
    {
        [Key]
        public virtual Guid RoleId { get; set; }

        //[Required]
        public string RoleName { get; set; }

        public string Description { get; set; }

        public IList<User> Users { get; set; }
    }
}