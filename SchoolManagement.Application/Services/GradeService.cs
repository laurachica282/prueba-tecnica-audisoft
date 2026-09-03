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
    public class GradeService : IGradeService
    {
        private readonly IGradeRepository _repository;
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;

        public GradeService(
            IGradeRepository repository,
            IStudentRepository studentRepository,
            ITeacherRepository teacherRepository)
        {
            _repository = repository;
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
        }

        public async Task<PagedResult<GradeDtos>> GetPagedAsync(
            PaginationQuery query, CancellationToken cancellationToken = default)
        {
            var (grades, totalCount) = await _repository.GetPagedAsync(query, cancellationToken);

            return new PagedResult<GradeDtos>
            {
                Items = grades.Select(MapToDto).ToList(),
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<GradeDtos> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var grade = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró la nota con id {id}.");

            return MapToDto(grade);
        }

        public async Task<GradeDtos> CreateAsync(
            CreateGradeDto dto, CancellationToken cancellationToken = default)
        {
            await ValidateReferencesAsync(dto.StudentId, dto.TeacherId, cancellationToken);

            var grade = new Grade
            {
                Name = dto.Name.Trim(),
                Value = dto.Value,
                StudentId = dto.StudentId,
                TeacherId = dto.TeacherId
            };

            await _repository.AddAsync(grade, cancellationToken);

            return await GetByIdAsync(grade.Id, cancellationToken);
        }

        public async Task<GradeDtos> UpdateAsync(
            int id, UpdateGradeDto dto, CancellationToken cancellationToken = default)
        {
            var grade = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró la nota con id {id}.");

            await ValidateReferencesAsync(dto.StudentId, dto.TeacherId, cancellationToken);

            grade.Name = dto.Name.Trim();
            grade.Value = dto.Value;
            grade.StudentId = dto.StudentId;
            grade.TeacherId = dto.TeacherId;

            await _repository.UpdateAsync(grade, cancellationToken);

            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var grade = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró la nota con id {id}.");

            await _repository.DeleteAsync(grade, cancellationToken);
        }

        private async Task ValidateReferencesAsync(
            int studentId, int teacherId, CancellationToken cancellationToken)
        {
            if (!await _studentRepository.ExistsAsync(studentId, cancellationToken))
                throw new NotFoundException($"No se encontró el estudiante con id {studentId}.");

            if (!await _teacherRepository.ExistsAsync(teacherId, cancellationToken))
                throw new NotFoundException($"No se encontró el profesor con id {teacherId}.");
        }

        private static GradeDtos MapToDto(Grade grade) => new()
        {
            Id = grade.Id,
            Name = grade.Name,
            Value = grade.Value,
            StudentId = grade.StudentId,
            StudentName = grade.Student?.Name ?? string.Empty,
            TeacherId = grade.TeacherId,
            TeacherName = grade.Teacher?.Name ?? string.Empty
        };
    }
}
