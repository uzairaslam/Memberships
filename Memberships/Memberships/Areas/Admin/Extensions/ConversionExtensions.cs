using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
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
        public static async Task<IEnumerable<ProductItemModel>> Convert(this IQueryable<ProductItem> productItems,
            ApplicationDbContext db)
        {
            if (productItems.Count().Equals(0)) return new List<ProductItemModel>();



            return await (from p in productItems
                select new ProductItemModel
                {
                    ItemId = p.ItemId,
                    ProductId = p.ProductId,
                    ItemTitle = db.Items.FirstOrDefault(i => i.Id.Equals(p.ItemId)).Title,
                    ProductTitle = db.Products.FirstOrDefault(i => i.Id.Equals(p.ProductId)).Title
                }).ToListAsync();
        }
        public static async Task<IEnumerable<SubscriptionProductModel>> Convert(this IQueryable<SubscriptionProduct> subscriptionProducts,
            ApplicationDbContext db)
        {
            if (subscriptionProducts.Count().Equals(0)) return new List<SubscriptionProductModel>();



            return await (from p in subscriptionProducts
                          select new SubscriptionProductModel
                {
                    SubscriptionId = p.SubscriptionId,
                    ProductId = p.ProductId,
                    SubscriptionTitle = db.Subscriptions.FirstOrDefault(i => i.Id.Equals(p.SubscriptionId)).Title,
                    ProductTitle = db.Products.FirstOrDefault(i => i.Id.Equals(p.ProductId)).Title
                }).ToListAsync();
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
        public static async Task<ProductItemModel> Convert(this ProductItem product,
            ApplicationDbContext db, bool addListData = true)
        {

            var model = new ProductItemModel()
                {
                    ItemId = product.ItemId,
                    ProductId = product.ProductId,
                    Items = addListData ? await db.Items.ToListAsync() : null,
                    Products = addListData ? await db.Products.ToListAsync() : null,
                    ItemTitle = (await db.Items.FirstOrDefaultAsync(i => i.Id.Equals(product.ItemId))).Title,
                    ProductTitle = (await db.Products.FirstOrDefaultAsync(i => i.Id.Equals(product.ProductId))).Title
                };

            return model;
        }
        public static async Task<SubscriptionProductModel> Convert(this SubscriptionProduct product,
            ApplicationDbContext db, bool addListData = true)
        {

            var model = new SubscriptionProductModel()
                {
                    SubscriptionId = product.SubscriptionId,
                    ProductId = product.ProductId,
                    Subscriptions = addListData ? await db.Subscriptions.ToListAsync() : null,
                    Products = addListData ? await db.Products.ToListAsync() : null,
                    SubscriptionTitle = (await db.Subscriptions.FirstOrDefaultAsync(i => i.Id.Equals(product.SubscriptionId))).Title,
                    ProductTitle = (await db.Products.FirstOrDefaultAsync(i => i.Id.Equals(product.ProductId))).Title
                };

            return model;
        }

        public static async Task<bool> CanChange(this ProductItem prd, ApplicationDbContext db)
        {
            var oldProduct =await db.ProductItems.CountAsync(pi =>
                pi.ItemId.Equals(prd.OldItemId) && pi.ProductId.Equals(prd.OldProductId));
            
            var newProduct =await db.ProductItems.CountAsync(pi =>
                pi.ItemId.Equals(prd.ItemId) && pi.ProductId.Equals(prd.ProductId));
            
            return oldProduct.Equals(1) && newProduct.Equals(0);
        }
        public static async Task<bool> CanChange(this SubscriptionProduct prd, ApplicationDbContext db)
        {
            var oldProduct =await db.SubscriptionProducts.CountAsync(pi =>
                pi.SubscriptionId.Equals(prd.OldSubscritionId) && pi.ProductId.Equals(prd.OldProductId));

            var newProduct = await db.SubscriptionProducts.CountAsync(pi =>
                pi.SubscriptionId.Equals(prd.SubscriptionId) && pi.ProductId.Equals(prd.ProductId));
            
            return oldProduct.Equals(1) && newProduct.Equals(0);
        }

        public static async Task Change(this ProductItem prd, ApplicationDbContext db)
        {
            var oldProduct = await db.ProductItems.FirstOrDefaultAsync(pi =>
                pi.ItemId.Equals(prd.OldItemId) && pi.ProductId.Equals(prd.OldProductId));

            var newProduct = await db.ProductItems.FirstOrDefaultAsync(pi =>
                pi.ItemId.Equals(prd.ItemId) && pi.ProductId.Equals(prd.ProductId));

            if (newProduct == null && oldProduct != null)
            {
                newProduct = new ProductItem
                {
                    ProductId = prd.ProductId,
                    ItemId = prd.ItemId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.ProductItems.Remove(oldProduct);
                        db.ProductItems.Add(newProduct);

                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                    }
                }
            }
        }
        public static async Task Change(this SubscriptionProduct prd, ApplicationDbContext db)
        {
            var oldProduct = await db.SubscriptionProducts.FirstOrDefaultAsync(pi =>
                pi.SubscriptionId.Equals(prd.OldSubscritionId) && pi.ProductId.Equals(prd.OldProductId));

            var newProduct = await db.SubscriptionProducts.FirstOrDefaultAsync(pi =>
                pi.SubscriptionId.Equals(prd.SubscriptionId) && pi.ProductId.Equals(prd.ProductId));

            if (newProduct == null && oldProduct != null)
            {
                newProduct = new SubscriptionProduct
                {
                    ProductId = prd.ProductId,
                    SubscriptionId = prd.SubscriptionId
                };

                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        db.SubscriptionProducts.Remove(oldProduct);
                        db.SubscriptionProducts.Add(newProduct);

                        await db.SaveChangesAsync();
                        transaction.Complete();
                    }
                    catch
                    {
                        transaction.Dispose();
                    }
                }
            }
        }
    }
}