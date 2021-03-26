using HealthChecks.DB;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthChecks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly PostContext _ctx;


        public PostsController(PostContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IEnumerable<Post>> GetPosts()
        {
            return await _ctx.Posts
                .ToListAsync()
                .ConfigureAwait(false);
        }

        [HttpPost]
        public async Task AddPost(IEnumerable<Post> posts)
        {
            foreach (var p in posts)
            {
                _ctx.Posts.Add(p);
            }
            await _ctx.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
