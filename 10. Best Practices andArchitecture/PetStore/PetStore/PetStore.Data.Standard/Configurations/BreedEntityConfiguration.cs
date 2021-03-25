using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetStore.Common;
using PetStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Data.Configurations
{
    public class BreedEntityConfiguration : IEntityTypeConfiguration<Breed>
    {
        public void Configure(EntityTypeBuilder<Breed> builder)
        {
            builder.Property(b => b.Name)
                .HasMaxLength(GlobalConstants.BreedNameMaxLength)
                .IsUnicode(true);

            builder.Property(b => b.Description)
                .HasMaxLength(GlobalConstants.BreedDescriptionMaxLength)
                .IsUnicode();
        }
    }
}
