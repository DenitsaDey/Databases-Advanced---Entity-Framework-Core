using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.ServiceModels.Products.OutputModels
{
    public class ListAllProductsByNameServiceModel
    {
        public string BrandName { get; set; }

        public string ProductType { get; set; }

        public decimal Price { get; set; }
    }
}
