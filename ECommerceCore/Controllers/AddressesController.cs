using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECommerceCore.Controllers
{
    [ApiController]
    [Route("api/addresses")]
    public class AddressesController : ControllerBase
    {
        //Access to the DB
        private readonly ShopoContext shopoDbContext;

        //Constructor
        public AddressesController(ShopoContext shopoDbContext) => this.shopoDbContext = shopoDbContext;


        public class AddressResponse
        {
            public List<Addresses> Addresses { get; set; }
            public bool HasAddress { get; set; }
        }

        // GET api/<AddressController>/5
        [HttpGet]
        public async Task<AddressResponse> Get([FromQuery] int User_Id)
        {
            var response = new AddressResponse();
            var address = await shopoDbContext.Addresses.Where(a => a.UserId == User_Id).ToListAsync();

            if (address.Count == 0)
            {
                response.HasAddress = false;
            }
            else
            {
                response.HasAddress = true;
            }
            response.Addresses = address;

            return response;

        }

        public class PostResponse
        {
            public int Id { get; set; }
            public bool Action { get; set; }
        }

        // POST api/<AddressController>
        [HttpPost]
        public async Task<PostResponse> Post([FromBody] Addresses address)
        {
            var pr = new PostResponse();

            try
            {
                if (address != null)
                {
                    shopoDbContext.Addresses.Add(address);
                    await shopoDbContext.SaveChangesAsync();
                    pr.Id = address.UserId;
                    pr.Action = true;
                }
                else
                {
                    pr.Action = false;
                }
            }
            catch { }

            return pr;
        }

        // PUT api/<AddressController>/5
        [HttpPut]
        public async Task<PostResponse> Put([FromQuery] int Address_Id, [FromBody] Addresses newAddress)
        {
            var pr = new PostResponse();

            try
            {
                var address = await shopoDbContext.Addresses.FirstOrDefaultAsync(a => a.AddressId == Address_Id);

                if (address != null)
                {
                    address.City = newAddress.City;
                    address.StreetAddress = newAddress.StreetAddress;
                    address.PostCode = newAddress.PostCode;
                    address.PhoneNumber = newAddress.PhoneNumber;
                    address.LastName = newAddress.LastName;
                    address.FirstName = newAddress.FirstName;

                    await shopoDbContext.SaveChangesAsync();
                    pr.Id = address.UserId;
                    pr.Action = true;
                }
                else
                {
                    pr.Action = false;
                }
            }
            catch { }

            return pr;
        }

        // DELETE api/<AddressController>/5
        [HttpDelete("{id}")]
        public async Task<PostResponse> Delete(int id)
        {
            var pr = new PostResponse();

            try
            {
                var address = await shopoDbContext.Addresses.FirstOrDefaultAsync(a => a.AddressId == id);

                if (address != null)
                {
                    shopoDbContext.Addresses.Remove(address);
                    await shopoDbContext.SaveChangesAsync();
                    pr.Action = true;
                }
                pr.Id = id;
            }

            catch { }

            return pr;
        }
    }
}
