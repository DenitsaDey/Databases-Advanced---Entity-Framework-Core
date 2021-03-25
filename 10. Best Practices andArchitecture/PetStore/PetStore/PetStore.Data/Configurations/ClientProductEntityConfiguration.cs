using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PetStore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.Data.Configurations
{
    public class ClientProductEntityConfiguration : IEntityTypeConfiguration<ClientProduct>
    {
        public void Configure(EntityTypeBuilder<ClientProduct> builder)
        {
            builder.HasKey(cp => new { cp.ClientId, cp.ProductId });

            builder.HasOne(cp => cp.Order)
                .WithMany(o => o.ClientProducts)
                .HasForeignKey(cp => cp.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
