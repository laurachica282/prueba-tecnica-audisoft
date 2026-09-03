using SchoolManagement.Application.Common;
using SchoolManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Interfaces
{
    public interface IStudentService
    {
        Task<PagedResult<StudentDtos>> GetPagedAsync(
        PaginationQuery query, CancellationToken cancellationToken = default);

        Task<StudentDtos> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<StudentDtos> CreateAsync(CreateStudentDto dto, CancellationToken cancellationToken = default);
        Task<StudentDtos> UpdateAsync(int id, UpdateStudentDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
