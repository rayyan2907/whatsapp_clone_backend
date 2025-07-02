using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using whatsapp_clone_backend.Controllers;
using whatsapp_clone_backend.Data;
using whatsapp_clone_backend.Models;
using whatsapp_clone_backend.Services;

namespace whatsapp_clone_backend.Hubs
{
    public class ChatHub : Hub
    {
        private readonly Message_DL _msg_dl;

        private static readonly ConcurrentDictionary<string, HashSet<string>> _chatConnections = new();

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("[ChatHub] OnConnectedAsync triggered");

            var userIdClaim = Context.User?.FindFirst("user_id"); // get from JWT token

            if (userIdClaim == null)
            {
                Console.WriteLine("[ChatHub] Unauthorized connection attempt — no user_id claim");
                Context.Abort(); // Close connection if JWT is invalid
                return;
            }

            var userId = userIdClaim.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("[ChatHub] Invalid user_id");
                Context.Abort();
                return;
            }

            lock (_chatConnections)
            {
                if (!_chatConnections.ContainsKey(userId))
                    _chatConnections[userId] = new HashSet<string>();

                _chatConnections[userId].Add(Context.ConnectionId);
            }

            Console.WriteLine($"[ChatHub] User {userId} connected with ConnectionId: {Context.ConnectionId}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.GetHttpContext()?.Request.Query["user_id"];
            bool shouldCleanup = false;

            if (!string.IsNullOrEmpty(userId))
            {
                lock (_chatConnections)
                {
                    if (_chatConnections.TryGetValue(userId, out var connections))
                    {
                        connections.Remove(Context.ConnectionId);
                        if (connections.Count == 0)
                        {
                            _chatConnections.TryRemove(userId, out _);
                            shouldCleanup = true;
                        }
                    }
                }

                if (shouldCleanup)
                {
                    Console.WriteLine($"[ChatHub] {userId} disconnected (Chat)");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessage(Message_recieve_model message)
        {
            MessageService _service = new MessageService(_msg_dl);

            if (message.type == "img")
            {
                    
            }
            else if (message.type == "video")
            {

            }
            else if (message.type == "voice")
            {

            }
            else if(message.type=="msg") 
            {

            }





            // Send to recipient
            if (_chatConnections.TryGetValue(message.sender_id.ToString(), out var connections))
            {
                foreach (var conn in connections)
                {
                    await Clients.Client(conn).SendAsync("ReceiveMessage", message);
                }
            }

            // Also send to sender (for echo)
            if (_chatConnections.TryGetValue(message.reciever_id.ToString(), out var senderConnections))
            {
                foreach (var conn in senderConnections)
                {
                    await Clients.Client(conn).SendAsync("ReceiveMessage", message);
                }
            }

            Console.WriteLine($"Message sent: {message.text_msg}");
        }


    }
}
