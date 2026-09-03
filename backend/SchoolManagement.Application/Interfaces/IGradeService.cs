using SchoolManagement.Application.Common;
using SchoolManagement.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Interfaces
{
    public interface IGradeService
    {
        Task<PagedResult<GradeDtos>> GetPagedAsync(
        PaginationQuery query, CancellationToken cancellationToken = default);

        Task<GradeDtos> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<GradeDtos> CreateAsync(CreateGradeDto dto, CancellationToken cancellationToken = default);
        Task<GradeDtos> UpdateAsync(int id, UpdateGradeDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
