﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.ModelConfigurations
{
    public class HomeworkConfiguration : IEntityTypeConfiguration<Homework>
    {
        public void Configure(EntityTypeBuilder<Homework> homework)
        {
            homework.HasKey(h => h.HomeworkId);

            homework.Property(h => h.Content)
                .IsUnicode(false)
                .IsRequired();

            homework.Property(h => h.ContentType)
                .IsRequired();

            homework.Property(h => h.SubmissionTime)
                .IsRequired();

            homework.HasOne(h => h.Student)
                .WithMany(s => s.HomeworkSubmissions)
                .HasForeignKey(h => h.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            homework.HasOne(h => h.Course)
                .WithMany(c => c.HomeworkSubmissions)
                .HasForeignKey(h => h.CourseId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
