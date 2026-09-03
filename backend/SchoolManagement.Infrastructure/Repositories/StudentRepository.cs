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
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IReadOnlyList<Student> Items, int TotalCount)> GetPagedAsync(
    PaginationQuery query, CancellationToken cancellationToken = default)
        {
            var baseQuery = _context.Students.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim();
                baseQuery = baseQuery.Where(s => s.Name.Contains(term));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            baseQuery = query.SortBy?.ToLowerInvariant() switch
            {
                "id" => query.IsDescending
                    ? baseQuery.OrderByDescending(s => s.Id)
                    : baseQuery.OrderBy(s => s.Id),
                _ => query.IsDescending
                    ? baseQuery.OrderByDescending(s => s.Name)
                    : baseQuery.OrderBy(s => s.Name)
            };

            var items = await baseQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _context.Students.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
            => await _context.Students.AnyAsync(s => s.Id == id, cancellationToken);

        public async Task<int> CountGradesAsync(int studentId, CancellationToken cancellationToken = default)
            => await _context.Grades.CountAsync(g => g.StudentId == studentId, cancellationToken);

        public async Task<Dictionary<int, int>> GetGradeCountsAsync(
            IEnumerable<int> studentIds, CancellationToken cancellationToken = default)
        {
            var ids = studentIds.ToList();
            if (ids.Count == 0) return new Dictionary<int, int>();

            return await _context.Grades
                .Where(g => ids.Contains(g.StudentId))
                .GroupBy(g => g.StudentId)
                .Select(g => new { StudentId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.StudentId, x => x.Count, cancellationToken);
        }

        public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Student student, CancellationToken cancellationToken = default)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
