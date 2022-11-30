using System.ComponentModel.DataAnnotations;

namespace PPTBoardAPI.DTOs
{
    public class UserRegisterCredentials
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Fio { get; set; }
    }
}
