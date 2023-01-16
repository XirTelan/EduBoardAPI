using Microsoft.AspNetCore.Identity;
using PPTBoardAPI.Entities;

namespace PPTBoardAPI.Authentication
{
    public class Person : IdentityUser
    {
        public string Fio { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public List<Group>? Groups { get; set; }
    }
}
