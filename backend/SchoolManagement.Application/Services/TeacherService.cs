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
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _repository;

        public TeacherService(ITeacherRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<TeacherDtos>> GetPagedAsync(PaginationQuery query, CancellationToken cancellationToken = default)
        {
            var (teachers, totalCount) = await _repository.GetPagedAsync(query, cancellationToken);
            var ids = teachers.Select(s => s.Id).ToList();

            var gradeCounts = await _repository.GetGradeCountsAsync(ids, cancellationToken);
            var studentsCounts = await _repository.GetDistinctStudentCountsAsync(ids, cancellationToken);
            var totalStudents = await _repository.CountAllStudentsAsync(cancellationToken);

            var items = teachers
                .Select(t => MapToDto(t, gradeCounts.GetValueOrDefault(t.Id),
                studentsCounts.GetValueOrDefault(t.Id),
                totalStudents))
                .ToList();

            return new PagedResult<TeacherDtos>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        private static TeacherDtos MapToDto(Teacher teacher, int gradeCount, int distinctTeacherCount, int totalTeachers) => new()
        {
            Id = teacher.Id,
            Name = teacher.Name,
            GradeCount = gradeCount,
            DistinctStudentCount = distinctTeacherCount,
            TotalStudents = totalTeachers
        };

        public async Task<TeacherDtos> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var teacher = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el profesor con id {id}.");

            var gradeCounts = await _repository.CountGradesAsync(id, cancellationToken);
            var studentsCounts = await _repository.GetDistinctStudentCountsAsync([id], cancellationToken);
            var totalStudents = await _repository.CountAllStudentsAsync(cancellationToken);

            return MapToDto(teacher, gradeCounts, studentsCounts.GetValueOrDefault(id), totalStudents);
        }

        public async Task<TeacherDtos> CreateAsync(CreateTeacherDto dto, CancellationToken cancellationToken = default)
        {
            var teacher = new Teacher { Name = dto.Name.Trim() };
            await _repository.AddAsync(teacher, cancellationToken);

            var totalStudents = await _repository.CountAllStudentsAsync(cancellationToken);
            return MapToDto(teacher, 0,0, totalStudents);
        }

        public async Task<TeacherDtos> UpdateAsync(int id, UpdateTeacherDto dto, CancellationToken cancellationToken = default)
        {
            var teacher = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el profesor con id {id}.");

            teacher.Name = dto.Name.Trim();
            await _repository.UpdateAsync(teacher, cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var teacher = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el profesor con id {id}.");

            var gradeCount = await _repository.CountGradesAsync(id, cancellationToken);

            if (gradeCount > 0)
            {
                throw new ConflictException(
                    $"No se puede eliminar el profesor \"{teacher.Name}\" " +
                    $"porque tiene {gradeCount} nota(s) asociada(s).");
            }

            await _repository.DeleteAsync(teacher, cancellationToken);
        }

        public async Task<IReadOnlyList<TeacherDtos>> GetLookupAsync(CancellationToken cancellationToken = default)
        {
            var teachers = await _repository.GetAllAsync(cancellationToken);

            return teachers
                .Select(s => new TeacherDtos { Id = s.Id, Name = s.Name })
                .ToList();
        }
    }
}
