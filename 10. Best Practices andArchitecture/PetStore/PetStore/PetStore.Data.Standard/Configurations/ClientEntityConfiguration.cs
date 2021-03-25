using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetStore.Common;
using PetStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Data.Configurations
{
    public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.Property(c => c.Name)
                .HasMaxLength(GlobalConstants.ClientNameMaxLength)
                .IsUnicode();

            builder.Property(c => c.Email)
                .HasMaxLength(GlobalConstants.EmailMaxLength)
                .IsUnicode(false);
        }
    }
}
