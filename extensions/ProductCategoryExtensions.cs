using PeyulErp.Contracts;
using PeyulErp.Models;

namespace PeyulErp.Extensions{
    public static class ProductCategoryExtensions{
        public static GetProductCategoryDTO AsDTO(this ProductCategory category){
            return new GetProductCategoryDTO {
                Id = category.Id,
                Name = category.Name,
                CreatedDate = category.CreateDate,
                UpdatedDate = category.UpdateDate
            };
        }

        public static ProductCategory AsInternal(this ProductCategoryDTO categoryDTO){
            return new ProductCategory {
                Id = Guid.NewGuid(),
                Name = categoryDTO.Name,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
        }
    }
}