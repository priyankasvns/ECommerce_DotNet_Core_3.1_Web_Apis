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
    [ApiController]
    [Route("api/comment")]
    public class CommentsController : ControllerBase
    {
        //Access to the DB
        private readonly ShopoContext shopoDbContext;

        //Constructor
        public CommentsController(ShopoContext shopoDbContext) => this.shopoDbContext = shopoDbContext;

        public class ProductCommentResponse
        {
            public List<Comments> Comments { get; set; }
            public bool HasComments { get; set; }
        }

        // GET api/<CommentController>/
        [HttpGet]
        public async Task<ProductCommentResponse> Get([FromQuery] int Product_Id)
        {
            var response = new ProductCommentResponse();
            var comment = await shopoDbContext.Comments.Where(c => c.ProductId == Product_Id).ToListAsync();

            if (comment.Count == 0)
            {
                response.HasComments = false;
            }
            else
            {
                response.HasComments = true;
            }
            response.Comments = comment;

            return response;
        }

        public class PostResponse
        {
            public int Id { get; set; }
            public bool ActionSuccessfully { get; set; }
        }

        // POST api/<CommentController>
        [HttpPost]
        public async Task<PostResponse> Post([FromQuery] int User_Id, [FromQuery] int Product_Id, [FromBody] Comments comment)
        {
            var pr = new PostResponse();

            try
            {
                comment.UserId = User_Id;
                comment.ProductId = Product_Id;
                comment.CreatedDate = DateTime.Now;
                comment.ModifiedDate = DateTime.Now;
                shopoDbContext.Comments.Add(comment);
                await shopoDbContext.SaveChangesAsync();

                pr.Id = comment.CommentId;
                pr.ActionSuccessfully = true;
            }
            catch { }

            return pr;
        }

        // PUT api/<CommentController>/5
        [HttpPut]
        public async Task<PostResponse> Put([FromQuery] int Comment_Id, [FromBody] Comments newComment)
        {
            var pr = new PostResponse();

            try
            {
                var comment = await shopoDbContext.Comments.FirstOrDefaultAsync(a => a.CommentId == Comment_Id);

                if (comment != null)
                {
                    comment.CommentImage = newComment.CommentImage;
                    comment.Content = newComment.Content;
                    comment.ModifiedDate = DateTime.Now;

                    await shopoDbContext.SaveChangesAsync();
                    pr.Id = comment.CommentId;
                    pr.ActionSuccessfully = true;
                }
                else
                {
                    pr.ActionSuccessfully = false;
                }
            }
            catch { }

            return pr;
        }

        // DELETE api/<CommentController>/5
        [HttpDelete]
        public async Task<PostResponse> Delete([FromQuery] int Comment_Id, [FromQuery] int User_Id)
        {
            var pr = new PostResponse();

            try
            {
                var comment = await shopoDbContext.Comments.FirstOrDefaultAsync(c => c.CommentId == Comment_Id && c.UserId == User_Id);

                if (comment != null)
                {
                    shopoDbContext.Comments.Remove(comment);
                    await shopoDbContext.SaveChangesAsync();
                    pr.ActionSuccessfully = true;
                }
                pr.Id = Comment_Id;
            }

            catch { }

            return pr;
        }
    }
}
