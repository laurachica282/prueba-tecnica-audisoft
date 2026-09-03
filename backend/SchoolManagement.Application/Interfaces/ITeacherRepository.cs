using SchoolManagement.Application.Common;
using SchoolManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Interfaces
{
    public interface ITeacherRepository
    {
        Task<(IReadOnlyList<Teacher> Items, int TotalCount)> GetPagedAsync(
        PaginationQuery query, CancellationToken cancellationToken = default);

        Task<Teacher?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

        Task<int> CountGradesAsync(int teacherId, CancellationToken cancellationToken = default);

        Task<Dictionary<int, int>> GetGradeCountsAsync(
            IEnumerable<int> teacherIds, CancellationToken cancellationToken = default);

        Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default);
        Task UpdateAsync(Teacher teacher, CancellationToken cancellationToken = default);
        Task DeleteAsync(Teacher teacher, CancellationToken cancellationToken = default);
        Task<Dictionary<int, int>> GetDistinctStudentCountsAsync(
    IEnumerable<int> teacherIds, CancellationToken cancellationToken = default);

        Task<int> CountAllStudentsAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Teacher>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
