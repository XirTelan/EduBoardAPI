﻿using System.ComponentModel.DataAnnotations;

namespace PPTBoardAPI.Entities
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string? MiddleName { get; set; }
        public int? GroupId { get; set; }

        public Group? Group { get; set; }
        public List<Attendance> Attendances { get; set; }

    }
}
