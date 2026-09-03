using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common;
using SchoolManagement.Application.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolManagement.Infrastructure.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly AppDbContext _context;

        public TeacherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IReadOnlyList<Teacher> Items, int TotalCount)> GetPagedAsync(
            PaginationQuery query, CancellationToken cancellationToken = default)
        {
            var baseQuery = _context.Teachers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim();
                baseQuery = baseQuery.Where(t => t.Name.Contains(term));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                .OrderBy(t => t.Name)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Teacher?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _context.Teachers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => await _context.Teachers.AnyAsync(t => t.Id == id, cancellationToken);

        public async Task<int> CountGradesAsync(int teacherId, CancellationToken cancellationToken = default)
            => await _context.Grades.CountAsync(g => g.TeacherId == teacherId, cancellationToken);

        public async Task<Dictionary<int, int>> GetGradeCountsAsync(
            IEnumerable<int> teacherIds, CancellationToken cancellationToken = default)
        {
            var ids = teacherIds.ToList();
            if (ids.Count == 0) return new Dictionary<int, int>();

            return await _context.Grades
                .Where(g => ids.Contains(g.TeacherId))
                .GroupBy(g => g.TeacherId)
                .Select(g => new { TeacherId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TeacherId, x => x.Count, cancellationToken);
        }

        public async Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Teacher teacher, CancellationToken cancellationToken = default)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Teacher teacher, CancellationToken cancellationToken = default)
        {
            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
