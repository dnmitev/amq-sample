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
        const int PAGE_SIZE = 10;

        private readonly PostContext _ctx;


        public PostsController(PostContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet]
        public async Task<IEnumerable<Post>> GetPosts(int page = 1)
        {
            return await _ctx.Posts
                .Skip((page - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
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
