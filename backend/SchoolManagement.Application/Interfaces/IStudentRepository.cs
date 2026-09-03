using SchoolManagement.Application.Common;
using SchoolManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Application.Interfaces
{
    public interface IStudentRepository
    {
        Task<(IReadOnlyList<Student> Items, int TotalCount)> GetPagedAsync(
        PaginationQuery query, CancellationToken cancellationToken = default);

        Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
        Task<int> CountGradesAsync(int studentId, CancellationToken cancellationToken = default);
        Task<Dictionary<int, int>> GetGradeCountsAsync(IEnumerable<int> studentIds, CancellationToken cancellationToken = default);
        Task AddAsync(Student student, CancellationToken cancellationToken = default);
        Task UpdateAsync(Student student, CancellationToken cancellationToken = default);
        Task DeleteAsync(Student student, CancellationToken cancellationToken = default);
        Task<Dictionary<int, int>> GetDistinctTeacherCountsAsync(IEnumerable<int> studentIds, CancellationToken cancellationToken = default);
        Task<int> CountAllTeachersAsync(CancellationToken cancellationToken = default);
    }
}
