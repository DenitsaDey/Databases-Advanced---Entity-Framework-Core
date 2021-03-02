
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P01_StudentSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace P01_StudentSystem.Data.ModelConfigurations
{
    public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> resource)
        {
            resource.HasKey(r => r.ResourceId);

            resource.Property(r => r.Name)
                .HasMaxLength(50)
                .IsRequired()
                .IsUnicode();

            resource.Property(r => r.Url)
                .IsUnicode(false)
                .IsRequired();

            resource.Property(r => r.ResourceType)
                .IsRequired();

            resource.HasOne(r => r.Course)
                .WithMany(c => c.Resources)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
