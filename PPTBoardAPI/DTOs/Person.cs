using Microsoft.AspNetCore.Identity;
using PPTBoardAPI.Entities;

namespace PPTBoardAPI.DTOs
{
    public class Person : IdentityUser
    {
        public string Fio { get; set; }
        public List<Group> Groups { get; set; }
    }
}
