using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchoolManagement.Application.DTOs
{
    public class GradeDtos
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
    }

    public class CreateGradeDto
    {
        [Required(ErrorMessage = "El nombre de la nota es obligatorio.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 5, ErrorMessage = "La nota debe estar entre 0.0 y 5.0.")]
        public decimal Value { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estudiante.")]
        public int StudentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un profesor.")]
        public int TeacherId { get; set; }
    }

    public class UpdateGradeDto
    {
        [Required(ErrorMessage = "El nombre de la nota es obligatorio.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 5, ErrorMessage = "La nota debe estar entre 0.0 y 5.0.")]
        public decimal Value { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estudiante.")]
        public int StudentId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un profesor.")]
        public int TeacherId { get; set; }
    }
}
