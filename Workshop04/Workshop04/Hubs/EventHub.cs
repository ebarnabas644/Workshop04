using Microsoft.AspNetCore.SignalR;

namespace Workshop04.Hubs
{
    public class EventHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Clients.Caller.SendAsync("Connected", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {

            return base.OnDisconnectedAsync(exception);
        }
    }
}
