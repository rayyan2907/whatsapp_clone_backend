using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Linq;

namespace whatsapp_clone_backend.Hubs
{
    public class StatusHub : Hub
    {
        private static ConcurrentDictionary<string, HashSet<string>> _userConnections = new();

            
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("status tab called");
            var user_id = Context.GetHttpContext()?.Request.Query["user_id"];
            if (!string.IsNullOrEmpty(user_id))
            {
                lock (_userConnections)
                {
                    if (!_userConnections.ContainsKey(user_id))
                        _userConnections[user_id] = new HashSet<string>();

                    _userConnections[user_id].Add(Context.ConnectionId);
                }
                Console.WriteLine(user_id + "is online");
                // Notify others that this user is online
                await Clients.Others.SendAsync("UserStatusChanged", user_id, true);
                
               
            }
            

            await base.OnConnectedAsync();
        }
        public static bool IsUserOnline(string userId)
        {
            lock (_userConnections)
            {
                return _userConnections.ContainsKey(userId);
            }
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user_id = Context.GetHttpContext()?.Request.Query["user_id"];
            bool shouldNotifyOffline = false;

            if (!string.IsNullOrEmpty(user_id))
            {
                lock (_userConnections)
                {
                    if (_userConnections.ContainsKey(user_id))
                    {
                        _userConnections[user_id].Remove(Context.ConnectionId);

                        if (_userConnections[user_id].Count == 0)
                        {
                            _userConnections.TryRemove(user_id, out _);
                            shouldNotifyOffline = true; // defer the await outside lock
                        }
                    }
                }

                if (shouldNotifyOffline)
                {
                    Console.WriteLine($"{user_id} is now offline");
                    await Clients.Others.SendAsync("UserStatusChanged", user_id, false);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
iterator 

    }

}
