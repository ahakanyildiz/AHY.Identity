using System.ComponentModel.DataAnnotations;

namespace AHY.Identity.Models
{
    public class UserAdminCreateModel
    {
        [Required(ErrorMessage ="Kullanıcı adı gereklidir.")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="Email alanı gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage ="Cinsiyet gereklidir.")]
        public string Gender { get; set; }
    }
}
