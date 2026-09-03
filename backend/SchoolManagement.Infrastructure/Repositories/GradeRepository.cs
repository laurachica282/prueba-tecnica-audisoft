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
    public class GradeRepository : IGradeRepository
    {
        private readonly AppDbContext _context;

        public GradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IReadOnlyList<Grade> Items, int TotalCount)> GetPagedAsync(
            PaginationQuery query, CancellationToken cancellationToken = default)
        {
            IQueryable<Grade> filtered = _context.Grades
                .AsNoTracking()
                .Include(g => g.Student)
                .Include(g => g.Teacher);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var term = query.Search.Trim();
                filtered = filtered.Where(g =>
                    g.Name.Contains(term) ||
                    g.Student.Name.Contains(term) ||
                    g.Teacher.Name.Contains(term));
            }

            var totalCount = await filtered.CountAsync(cancellationToken);

            var items = await filtered
                .OrderBy(g => g.Student.Name)
                .ThenBy(g => g.Name)
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task<Grade?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Teacher)
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        public async Task AddAsync(Grade grade, CancellationToken cancellationToken = default)
        {
            _context.Grades.Add(grade);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Grade grade, CancellationToken cancellationToken = default)
        {
            _context.Grades.Update(grade);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Grade grade, CancellationToken cancellationToken = default)
        {
            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
