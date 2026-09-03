using SchoolManagement.Application.Common;
using SchoolManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Interfaces
{
    public interface ITeacherService
    {
        Task<PagedResult<TeacherDtos>> GetPagedAsync(
        PaginationQuery query, CancellationToken cancellationToken = default);

        Task<TeacherDtos> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TeacherDtos> CreateAsync(CreateTeacherDto dto, CancellationToken cancellationToken = default);
        Task<TeacherDtos> UpdateAsync(int id, UpdateTeacherDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
