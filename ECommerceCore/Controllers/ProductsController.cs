using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ECommerceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ShopoContext _shopoContext;


        public class ProductFetchResponse
        {
            public bool Success { get; set; }
            public List<Products> products { get; set; }
        }

        public class SingleProductFetchResponse
        {
            public bool Success { get; set; }
            public Products products { get; set; }
        }
        public class ProductPostResponse
        {
            public bool Success { get; set; }
            public Products product { get; set; }
        }

        public class ProductRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Size { get; set; }
            public string Color { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int AvailableQuantity { get; set; }
            public string Picture { get; set; }

        }

        public class DeleteRequest
        {
            public int ProductId { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }
        public class DeleteResponse
        {
            public bool IsProductDeleted { get; set; }
            public bool IsAdmin { get; set; }
            public bool IsValidUserRequestData { get; set; }
        }

        public ProductsController(ShopoContext shopoContext)
        {
            _shopoContext = shopoContext;

        }
        // GET: api/Products
        [HttpGet]
        public async Task<ProductFetchResponse> Get()
        {
            var response = new ProductFetchResponse();
            var products = await _shopoContext.Products
                .Select(p => new Products { ProductId = p.ProductId, Size = p.Size, Color = p.Color, Name = p.Name, Description = p.Description, Price = p.Price, AvailableQuantity = p.AvailableQuantity, Picture = p.Picture }).ToListAsync();
            if (products.Count > 0)
            {
                response.products = products;
                response.Success = true;
            }
            else
            {
                response.products = null;
                response.Success = false;
            }

            return response;
        }

        // GET: api/Products/{productId}
        [HttpGet("{productId}")]
        public async Task<SingleProductFetchResponse> GetProductById(int productId)
        {

            var response = new SingleProductFetchResponse();
            var product = await _shopoContext.Products
                .Select(p => new Products { ProductId = p.ProductId, Size = p.Size, Color = p.Color, Name = p.Name, Description = p.Description, Price = p.Price, AvailableQuantity = p.AvailableQuantity, Picture = p.Picture })
                .Where(p => p.ProductId == productId)
                .FirstOrDefaultAsync();
            if (product != null)
            {
                response.products = product;
                response.Success = true;
            }
            else
            {
                response.products = null;
                response.Success = false;
            }
            return response;
        }

        // POST: api/Products
        [HttpPost]
        public async Task<ProductPostResponse> Post([FromBody] ProductRequest products)
        {
            var response = new ProductPostResponse();

            if (new EmailAddressAttribute().IsValid(products.Email) && products.Password != "")
            {
                var passwordBytes = System.Text.Encoding.UTF8.GetBytes(products.Password);
                var user = (from u in _shopoContext.Users
                            where u.Email == products.Email && u.Password == passwordBytes
                            select new Users { UserId = u.UserId }).ToList();
                if (user.Count > 0 && user[0].UserId == 16)
                {
                    Products product = new Products();
                    product.Size = products.Size;
                    product.Color = products.Size;
                    product.Name = products.Name;
                    product.Description = products.Description;
                    product.Price = products.Price;
                    product.AvailableQuantity = products.AvailableQuantity;
                    var encodedPicture = Convert.FromBase64String(products.Picture);
                    product.Picture = encodedPicture;
                    response.product = product;
                    if (response.product != null)
                    {
                        response.Success = true;
                        _shopoContext.Products.Add(product);
                        await _shopoContext.SaveChangesAsync();
                    }
                }
                else
                {
                    NotFound();
                }
            }
            else
            {
                response.product = new Products();
                response.Success = false;
            }
            return response;

        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        public async Task<DeleteResponse> Delete([FromBody] DeleteRequest request)
        {
            var response = new DeleteResponse();

            if (request.Email != null && request.Password != null)
            {
                if (new EmailAddressAttribute().IsValid(request.Email))
                {
                    response.IsValidUserRequestData = true;
                    var passwordBytes = System.Text.Encoding.UTF8.GetBytes(request.Password);
                    var user = (from u in _shopoContext.Users
                                where u.Email == request.Email && u.Password == passwordBytes
                                select new Users { UserId = u.UserId }).ToList();
                    if (user.Count > 0 && user[0].UserId == 16)
                    {
                        response.IsAdmin = true;
                        var product = await (from products in _shopoContext.Products
                                             where products.ProductId == request.ProductId
                                             orderby products.ProductId descending
                                             select products).FirstOrDefaultAsync();
                        if (product != null)
                        {
                            _shopoContext.Products.Remove(product);
                            await _shopoContext.SaveChangesAsync();
                            response.IsProductDeleted = true;
                        }
                        else
                        {
                            response.IsProductDeleted = false;
                        }
                    }
                    else
                    {
                        response.IsAdmin = false;
                        NotFound();
                    }
                }
                else
                {
                    BadRequest();
                }
            }
            else
            {
                response.IsProductDeleted = false;
                response.IsValidUserRequestData = false;
            }
            return response;

        }
    }
}
