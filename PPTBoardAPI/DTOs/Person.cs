using Microsoft.AspNetCore.Identity;

namespace PPTBoardAPI.DTOs
{
    public class Person: IdentityUser
    {
        public string Fio { get; set; }
    }
}
