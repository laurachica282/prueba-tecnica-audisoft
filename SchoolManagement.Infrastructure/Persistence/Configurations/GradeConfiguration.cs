using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Infrastructure.Persistence.Configurations
{
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.ToTable("Grades", t =>
                t.HasCheckConstraint("CK_Grades_Value", "[Value] >= 0 AND [Value] <= 5"));

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(e => e.Value)
                   .IsRequired()
                   .HasColumnType("decimal(4,2)");

            builder.HasOne(e => e.Student)
                   .WithMany(s => s.Grades)
                   .HasForeignKey(e => e.StudentId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .HasConstraintName("FK_Grades_Students");

            builder.HasOne(e => e.Teacher)
                   .WithMany(t => t.Grades)
                   .HasForeignKey(e => e.TeacherId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .HasConstraintName("FK_Grades_Teachers");

            builder.HasIndex(e => e.StudentId).HasDatabaseName("IX_Grades_StudentId");
            builder.HasIndex(e => e.TeacherId).HasDatabaseName("IX_Grades_TeacherId");
        }
    }
}
