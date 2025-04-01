using Microsoft.AspNetCore.SignalR;

namespace razorsignalr_newsmng.Hubs
{
    public class NewsHub : Hub
    {
        public async Task NotifyChange()
        {
            await Clients.All.SendAsync("Change");
        }
    }
}