using System;
using System.Collections.Generic;
using System.Text;

namespace PetStore.ServiceModels.Products.OutputModels
{
    public class ListAllProductsByProductTypeServiceModel
    {
        public string BrandName { get; set; }

        public decimal Price { get; set; }
    }
}
