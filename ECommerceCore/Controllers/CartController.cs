using System;
using System.Collections.Generic;
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
    public class CartController : ControllerBase
    {
        int availableProducts;
        int addressId;

        bool addressIdFetched = false;
        bool productIsAvailable = false;
        private readonly ShopoContext _context;

        public CartController(ShopoContext context)
        {
            _context = context;
        }

        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Carts>>> Post([FromBody] int productid, [FromQuery] int userId, [FromQuery] int quantity)
        {
            try
            {
                var products = (from p in _context.Products
                                where p.ProductId == productid
                                select new Products
                                {
                                    AvailableQuantity = p.AvailableQuantity
                                }).ToList();

                var address = (from a in _context.Addresses
                               where a.UserId == userId
                               select new Addresses
                               {
                                   AddressId = a.AddressId
                               }).ToList();

                foreach (var item in products)
                {
                    if (products != null)
                    {
                        productIsAvailable = true;
                        availableProducts = item.AvailableQuantity;

                    }
                }

                foreach (var item in address)
                {
                    if (address != null)
                    {
                        addressIdFetched = true;
                        addressId = item.AddressId;
                    }
                }

                var productsInCart = (from c in _context.Carts
                                      where c.ProductId == productid && c.UserId == userId && c.Quantity >= 1
                                      select new Carts
                                      {
                                          CartId = c.CartId,
                                          ProductId = c.ProductId,
                                          UserId = c.UserId,
                                          AddressId = c.AddressId,
                                          Quantity = c.Quantity,
                                          TimeAdded = c.TimeAdded
                                      }).ToList();

                if (productsInCart.Count > 0)
                {
                    foreach (var eachProduct in productsInCart)
                    {
                        if (eachProduct.Quantity >= 1)
                        {
                            int new_quantity = eachProduct.Quantity + quantity;
                            Carts cart = new Carts();
                            cart.CartId = eachProduct.CartId;
                            cart.ProductId = eachProduct.ProductId;
                            cart.UserId = eachProduct.UserId;
                            cart.AddressId = eachProduct.AddressId;
                            cart.Quantity = new_quantity;
                            cart.TimeAdded = DateTime.Now;

                            _context.Carts.Update(cart);
                            await _context.SaveChangesAsync();
                            return await _context.Carts.ToListAsync();

                        }
                    }
                    await _context.SaveChangesAsync();
                    return await _context.Carts.ToListAsync();
                }

                else
                {
                    if (productIsAvailable == true && availableProducts > 0 && addressIdFetched == true)
                    {
                        Carts cart = new Carts();
                        cart.ProductId = productid;
                        cart.UserId = userId;
                        cart.AddressId = addressId;
                        cart.Quantity = quantity;
                        cart.TimeAdded = DateTime.Now;

                        _context.Carts.Add(cart);
                        await _context.SaveChangesAsync();
                        return await _context.Carts.ToListAsync();
                    }
                    await _context.SaveChangesAsync();
                    return await _context.Carts.ToListAsync();
                }

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        // PUT: api/Cart/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        public async Task<ActionResult<IEnumerable<Carts>>> Delete([FromBody] int productid, [FromQuery] int userId)
        {
            try
            {
                var productsInCart = (from c in _context.Carts
                                      where c.ProductId == productid && c.UserId == userId
                                      select new Carts
                                      {
                                          CartId = c.CartId,
                                          ProductId = c.ProductId,
                                          UserId = c.UserId,
                                          AddressId = c.AddressId,
                                          Quantity = c.Quantity,
                                          TimeAdded = c.TimeAdded
                                      }).ToList();

                if (productsInCart != null)
                {
                    foreach (var eachProduct in productsInCart)
                    {
                        if (eachProduct.Quantity > 1)
                        {
                            int new_quantity = eachProduct.Quantity - 1;
                            Carts cart = new Carts();
                            cart.CartId = eachProduct.CartId;
                            cart.ProductId = eachProduct.ProductId;
                            cart.UserId = eachProduct.UserId;
                            cart.AddressId = eachProduct.AddressId;
                            cart.Quantity = new_quantity;
                            cart.TimeAdded = DateTime.Now;
                            _context.Carts.Update(cart);
                            await _context.SaveChangesAsync();
                            return await _context.Carts.ToListAsync();
                        }
                        else if (eachProduct.Quantity == 1)
                        {
                            _context.Carts.Remove(eachProduct);
                            await _context.SaveChangesAsync();
                            return await _context.Carts.ToListAsync();
                        }
                        else
                        {
                            return BadRequest("No product found with this product id in the users cart!");
                        }

                    }

                    await _context.SaveChangesAsync();
                    return await _context.Carts.ToListAsync();
                }

                return null;
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
