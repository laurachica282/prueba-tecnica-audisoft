using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Domain.Entities
{
    public class Grade
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }

        public int StudentId { get; set; }
        public int TeacherId { get; set; }

        public Student Student { get; set; } = null!;
        public Teacher Teacher { get; set; } = null!;
    }
}
