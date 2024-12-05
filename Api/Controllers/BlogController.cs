using Business.Services.Interfaces;
using Data.Payloads;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;

    public BlogController(IBlogService blogService)
    {
        _blogService = blogService;
    }

    // GET: api/<BlogController>
    [HttpGet]
    public async Task<IEnumerable<Post>> Get()
    {
        return await _blogService.GetAllPosts();
    }

    // GET api/<BlogController>/5
    [HttpGet("{id}")]
    public async Task<Post> Get(string id)
    {
        return await _blogService.GetPostById(id);
    }

    // POST api/<BlogController>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PostPayload post)
    {
        await _blogService.AddPost(post);
        return Ok();
    }

    // PUT api/<BlogController>/5
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Post post)
    {
        await _blogService.UpdatePost(post);
        return Ok();
    }

    // DELETE api/<BlogController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }
}