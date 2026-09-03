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

        public async Task<PagedResult<StudentDtos>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
        {
            var (students, totalCount) = await _repository.GetPagedAsync(query, cancellationToken);
            var ids = students.Select(s => s.Id).ToList();

            var gradeCounts = await _repository.GetGradeCountsAsync(ids, cancellationToken);
            var teacherCounts = await _repository.GetDistinctTeacherCountsAsync(ids, cancellationToken);
            var totalTeachers = await _repository.CountAllTeachersAsync(cancellationToken);

            var items = students
                .Select(s => MapToDto(
                    s,
                    gradeCounts.GetValueOrDefault(s.Id),
                    teacherCounts.GetValueOrDefault(s.Id),
                    totalTeachers))
                .ToList();

            return new PagedResult<StudentDtos>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        private static StudentDtos MapToDto( Student student, int gradeCount, int distinctTeacherCount, int totalTeachers) => new()
            {
                Id = student.Id,
                Name = student.Name,
                GradeCount = gradeCount,
                DistinctTeacherCount = distinctTeacherCount,
                TotalTeachers = totalTeachers
            };

        public async Task<StudentDtos> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var student = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el estudiante con id {id}.");

            var gradeCount = await _repository.CountGradesAsync(id, cancellationToken);
            var teacherCounts = await _repository.GetDistinctTeacherCountsAsync([id], cancellationToken);
            var totalTeachers = await _repository.CountAllTeachersAsync(cancellationToken);

            return MapToDto(student, gradeCount, teacherCounts.GetValueOrDefault(id), totalTeachers);
        }

        public async Task<StudentDtos> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default)
        {
            var student = new Student { Name = dto.Name.Trim() };
            await _repository.AddAsync(student, cancellationToken);

            var totalTeachers = await _repository.CountAllTeachersAsync(cancellationToken);
            return MapToDto(student, 0, 0, totalTeachers);
        }

        public async Task<StudentDtos> UpdateAsync(int id, UpdateStudentDto dto, CancellationToken cancellationToken = default)
        {
            var student = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el estudiante con id {id}.");

            student.Name = dto.Name.Trim();
            await _repository.UpdateAsync(student, cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
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

        public async Task<IReadOnlyList<StudentDtos>> GetLookupAsync(CancellationToken cancellationToken = default)
        {
            var students = await _repository.GetAllAsync(cancellationToken);

            return students
                .Select(s => new StudentDtos { Id = s.Id, Name = s.Name })
                .ToList();
        }
    }
}
