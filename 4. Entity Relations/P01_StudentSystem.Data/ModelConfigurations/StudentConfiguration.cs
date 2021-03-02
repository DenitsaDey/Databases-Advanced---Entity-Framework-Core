using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.ModelConfigurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
       public void Configure(EntityTypeBuilder<Student> student)
        {
            student.HasKey(s => s.StudentId);

            student.Property(s => s.Name)
            .HasMaxLength(100)
            .IsRequired()
            .IsUnicode();

            student.Property(s => s.PhoneNumber)
            .HasMaxLength(10)
            .IsUnicode(false)
            .IsRequired(false);

            student.Property(s => s.RegisteredOn)
                .IsRequired();

            student.Property(s => s.Birthday)
                .IsRequired(false);
        }
    }
}
