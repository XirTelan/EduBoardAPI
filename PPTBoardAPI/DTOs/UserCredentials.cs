using System.ComponentModel.DataAnnotations;

namespace PPTBoardAPI.DTOs
{
    public class UserCredentials
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
