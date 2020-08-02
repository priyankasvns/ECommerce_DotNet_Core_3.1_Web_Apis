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
    public class OrdersController : ControllerBase
    {
        private readonly ShopoContext _context;

        public OrdersController(ShopoContext context)
        {
            _context = context;
        }

        // GET: api/Orders/5
        [HttpGet]
        public async Task<ActionResult<List<Orders>>> Get([FromQuery] int userId)
        {
            var ordersOfUser = (from o in _context.Orders
                                where o.UserId == userId
                                select new Orders
                                {
                                    OrderReferenceId = o.OrderReferenceId,
                                    UserId = o.UserId,
                                    ProductId = o.ProductId,
                                    CartId = o.CartId,
                                    Quantity = o.Quantity,
                                    Status = o.Status,
                                    Amount = o.Amount,
                                    OrderDateTime = o.OrderDateTime,
                                    OrderId = o.OrderId
                                }).ToListAsync();

            return await ordersOfUser;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Orders>>> Post([FromBody] int userId)
        {
            bool orderPlaced = false;
            bool orderIdGenerated = false;
            bool orderIdFetched = false;

            try
            {

                var productsInCart = (from c in _context.Carts
                                      where c.UserId == userId
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
                        var products = (from p in _context.Products
                                        where p.ProductId == eachProduct.ProductId
                                        select new Products
                                        {
                                            ProductId = p.ProductId,
                                            Size = p.Size,
                                            Color = p.Color,
                                            Name = p.Name,
                                            Description = p.Description,
                                            Price = p.Price,
                                            AvailableQuantity = p.AvailableQuantity,
                                            Picture = p.Picture
                                        }).ToList();

                        var fetchingOrderId = (from o in _context.Orders
                                               where o.UserId == userId
                                               orderby o.OrderId descending
                                               select new Orders
                                               {
                                                   OrderId = o.OrderId
                                               }).ToList();

                        int remainingQuantity = products[0].AvailableQuantity - eachProduct.Quantity;

                        if (remainingQuantity > 0)
                        {
                            int newOrderId;

                            if (fetchingOrderId.Count > 0 && orderIdFetched == false)
                            {
                                newOrderId = fetchingOrderId[0].OrderId;
                                newOrderId += 1;
                                orderIdFetched = true;
                                orderIdGenerated = true;
                            }
                            else if (orderIdFetched == true && orderIdGenerated == true)
                            {
                                newOrderId = fetchingOrderId[0].OrderId;
                            }
                            else
                            {
                                newOrderId = 1;
                            }


                            Orders order = new Orders();
                            order.UserId = userId;
                            order.ProductId = eachProduct.ProductId;
                            order.CartId = eachProduct.CartId;
                            order.Quantity = eachProduct.Quantity;
                            order.Status = "order placed";
                            order.Amount = products[0].Price;
                            order.OrderDateTime = DateTime.Now;
                            order.OrderId = newOrderId;

                            _context.Orders.Add(order);
                            await _context.SaveChangesAsync();
                            orderPlaced = true;

                            Products updateProductsQty = new Products();
                            updateProductsQty.ProductId = eachProduct.ProductId;
                            updateProductsQty.Size = products[0].Size;
                            updateProductsQty.Color = products[0].Color;
                            updateProductsQty.Name = products[0].Name;
                            updateProductsQty.Description = products[0].Description;
                            updateProductsQty.Price = products[0].Price;
                            updateProductsQty.AvailableQuantity = remainingQuantity;
                            updateProductsQty.Picture = products[0].Picture;

                            _context.Products.Update(updateProductsQty);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            return BadRequest("sorry this product is not available");
                        }

                    }

                    return await _context.Orders.ToListAsync();
                }
                else
                {
                    return BadRequest("no products in the cart for this user id");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
