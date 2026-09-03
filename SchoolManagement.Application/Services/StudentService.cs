using SchoolManagement.Application.Common;
using SchoolManagement.Application.DTOs;
using SchoolManagement.Application.Exceptions;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<StudentDtos>> GetPagedAsync(
            PaginationQuery query, CancellationToken cancellationToken = default)
        {
            var (students, totalCount) = await _repository.GetPagedAsync(query, cancellationToken);

            var counts = await _repository.GetGradeCountsAsync(
                students.Select(s => s.Id), cancellationToken);

            var items = students
                .Select(s => MapToDto(s, counts.GetValueOrDefault(s.Id)))
                .ToList();

            return new PagedResult<StudentDtos>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<StudentDtos> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var student = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el estudiante con id {id}.");

            var gradeCount = await _repository.CountGradesAsync(id, cancellationToken);
            return MapToDto(student, gradeCount);
        }

        public async Task<StudentDtos> CreateAsync(
            CreateStudentDto dto, CancellationToken cancellationToken = default)
        {
            var student = new Student { Name = dto.Name.Trim() };
            await _repository.AddAsync(student, cancellationToken);
            return MapToDto(student, 0);
        }

        public async Task<StudentDtos> UpdateAsync(
            int id, UpdateStudentDto dto, CancellationToken cancellationToken = default)
        {
            var student = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el estudiante con id {id}.");

            student.Name = dto.Name.Trim();
            await _repository.UpdateAsync(student, cancellationToken);

            var gradeCount = await _repository.CountGradesAsync(id, cancellationToken);
            return MapToDto(student, gradeCount);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var student = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el estudiante con id {id}.");

            var gradeCount = await _repository.CountGradesAsync(id, cancellationToken);

            if (gradeCount > 0)
            {
                throw new ConflictException(
                    $"No se puede eliminar el estudiante \"{student.Name}\" " +
                    $"porque tiene {gradeCount} nota(s) asociada(s).");
            }

            await _repository.DeleteAsync(student, cancellationToken);
        }

        private static StudentDtos MapToDto(Student student, int gradeCount) => new()
        {
            Id = student.Id,
            Name = student.Name,
            GradeCount = gradeCount
        };
    }
}
