using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Memberships.Areas.Admin.Models;
using Memberships.Entities;
using Memberships.Models;

namespace Memberships.Areas.Admin.Extensions
{
    public static class ConversionExtensions
    {
        public static async Task<IEnumerable<ProductModel>> Convert(this IEnumerable<Product> products,
            ApplicationDbContext db)
        {
            if (products.Count().Equals(0)) return new List<ProductModel>();

            var texts = await db.ProductLinkTexts.ToListAsync();
            var types = await db.ProductTypes.ToListAsync();

            return from p in products
                select new ProductModel
                {
                    Id = p.Id,
                    ProductTypes = types,
                    ProductLinkTextId = p.ProductLinkTextId,
                    ProductTypeId = p.ProductTypeId,
                    ProductLinkTexts = texts,
                    Title = p.Title,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl
                };
        }
        public static async Task<ProductModel> Convert(this Product product,
            ApplicationDbContext db)
        {

            var text = await db.ProductLinkTexts.FirstOrDefaultAsync(p => p.Id == product.ProductLinkTextId);
            var type = await db.ProductTypes.FirstOrDefaultAsync(p => p.Id == product.ProductTypeId);

            var model = new ProductModel
                {
                    Id = product.Id,
                    ProductTypes = new List<ProductType>(),
                    ProductLinkTextId = product.ProductLinkTextId,
                    ProductTypeId = product.ProductTypeId,
                    ProductLinkTexts = new List<ProductLinkText>(),
                    Title = product.Title,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl
                };
            model.ProductLinkTexts.Add(text);
            model.ProductTypes.Add(type);
            return model;
        }
    }
}