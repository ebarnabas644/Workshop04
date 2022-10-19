using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Workshop04.Data;
using Workshop04.Hubs;
using Workshop04Models;

namespace Workshop04.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LogController : ControllerBase
    {
        IHubContext<EventHub> hub;
        ApiDbContext db;

        private readonly UserManager<IdentityUser> userManager;

        public LogController(IHubContext<EventHub> hub, ApiDbContext db, UserManager<IdentityUser> userManager)
        {
            this.hub = hub;
            this.db = db;
            this.userManager = userManager;
        }

        [HttpPost]
        public async void AddLog([FromBody] LoggingModel model)
        {
            model.Id = Guid.NewGuid().ToString();

            db.Logs.Add(model); ;
            db.SaveChanges();

            await hub.Clients.All.SendAsync("watchMessage", model);
        }
    }
}
