﻿namespace PPTBoardAPI.DTOs
{
    public class UserRoleDTO
    {
        public string UserId { get; set; }
        public string Role { get; set; }
        public bool isDeleteFlag { get; set; } = false;
    }
}
