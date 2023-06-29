using PeyulErp.Contracts;
using PeyulErp.Models;

namespace PeyulErp.Extensions{
    public static class ProductExtensions{
        public static GetProductDTO AsGetDTO(this Product product, ProductCategory category){
            return new GetProductDTO{
                Id = product.Id,
                Name = product.Name,
                BarCode = product.BarCode,
                BuyingPrice = product.BuyingPrice,
                SellingPrice = product.SellingPrice,
                ProductCategory = category.AsDTO(),
                CreatedDate = product.CreateDate,
                UpdatedDate = product.UpdateDate
            };
        }
        public static Product AsInternaProduct(this SaveProductDTO productDto){
            return new Product{
                Id = Guid.NewGuid(),
                Name = productDto.Name,
                BarCode = productDto.BarCode,
                BuyingPrice = productDto.BuyingPrice,
                SellingPrice = productDto.SellingPrice,
                CategoryId = productDto.CategoryId,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
        }
    }
}