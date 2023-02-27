using System.ComponentModel.DataAnnotations;

namespace PPTBoardAPI.Authentication
{
    public class UserRegisterCredentials
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Fio { get; set; }
        public List<string>? Roles { get; set; }
    }
}
