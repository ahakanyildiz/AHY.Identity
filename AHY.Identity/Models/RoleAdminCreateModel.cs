using System;
using System.ComponentModel.DataAnnotations;

namespace AHY.Identity.Models
{
    public class RoleAdminCreateModel
    {
        [Required(ErrorMessage ="Rol ismi gereklidir.")]
        public string Name { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;

    }
}
