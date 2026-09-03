using SchoolManagement.Application.Common;
using SchoolManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Interfaces
{
    public interface IGradeRepository
    {
        Task<(IReadOnlyList<Grade> Items, int TotalCount)> GetPagedAsync(
        PaginationQuery query, CancellationToken cancellationToken = default);

        Task<Grade?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task AddAsync(Grade grade, CancellationToken cancellationToken = default);
        Task UpdateAsync(Grade grade, CancellationToken cancellationToken = default);
        Task DeleteAsync(Grade grade, CancellationToken cancellationToken = default);
    }
}
