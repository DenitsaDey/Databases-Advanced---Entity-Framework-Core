using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.ModelConfigurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> course)
        {
            course.HasKey(c => c.CourseId);

            course.Property(c => c.Name)
                .HasMaxLength(80)
                .IsRequired()
                .IsUnicode();

            course.Property(c => c.Description)
                .IsUnicode()
                .IsRequired(false);

            course.Property(c => c.StartDate)
                .IsRequired();

            course.Property(c => c.EndDate)
                .IsRequired();

            course.Property(c => c.Price)
                .IsRequired();
        }
    }
}
