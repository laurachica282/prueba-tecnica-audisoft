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

        public async Task<PagedResult<TeacherDtos>> GetPagedAsync(
            PaginationQuery query, CancellationToken cancellationToken = default)
        {
            var (teachers, totalCount) = await _repository.GetPagedAsync(query, cancellationToken);

            var counts = await _repository.GetGradeCountsAsync(
                teachers.Select(t => t.Id), cancellationToken);

            var items = teachers
                .Select(t => MapToDto(t, counts.GetValueOrDefault(t.Id)))
                .ToList();

            return new PagedResult<TeacherDtos>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<TeacherDtos> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var teacher = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el profesor con id {id}.");

            var gradeCount = await _repository.CountGradesAsync(id, cancellationToken);
            return MapToDto(teacher, gradeCount);
        }

        public async Task<TeacherDtos> CreateAsync(
            CreateTeacherDto dto, CancellationToken cancellationToken = default)
        {
            var teacher = new Teacher { Name = dto.Name.Trim() };
            await _repository.AddAsync(teacher, cancellationToken);
            return MapToDto(teacher, 0);
        }

        public async Task<TeacherDtos> UpdateAsync(
            int id, UpdateTeacherDto dto, CancellationToken cancellationToken = default)
        {
            var teacher = await _repository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException($"No se encontró el profesor con id {id}.");

            teacher.Name = dto.Name.Trim();
            await _repository.UpdateAsync(teacher, cancellationToken);

            var gradeCount = await _repository.CountGradesAsync(id, cancellationToken);
            return MapToDto(teacher, gradeCount);
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

        private static TeacherDtos MapToDto(Teacher teacher, int gradeCount) => new()
        {
            Id = teacher.Id,
            Name = teacher.Name,
            GradeCount = gradeCount
        };
    }
}
