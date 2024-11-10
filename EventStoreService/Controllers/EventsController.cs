using EventStoreService.Data;
using EventStoreService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventStoreService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly EventStoreDbContext _dbContext;

        public EventsController(EventStoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<StoredEvent>>>GetAll()
        {
            return await _dbContext.StoredEvents.ToListAsync();
        }
    }
}
