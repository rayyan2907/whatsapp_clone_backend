using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;

namespace whatsapp_clone_backend.Hubs
{
    public class ChatHub : Hub
    {
        // Optional: Add a method to send messages
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
