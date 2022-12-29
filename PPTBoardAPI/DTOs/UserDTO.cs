namespace PPTBoardAPI.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Fio { get; set; }
        public List<string> Roles { get; set; }
    }
}
