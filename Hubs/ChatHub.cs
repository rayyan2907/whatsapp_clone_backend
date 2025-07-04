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

        public ChatHub(Message_DL msg_dl)
        {
            _msg_dl=msg_dl;
        }


        private static readonly ConcurrentDictionary<string, HashSet<string>> _chatConnections = new();

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine("[ChatHub] OnConnectedAsync triggered");

            var userIdClaim = Context.User?.FindFirst("user_id"); // get from JWT token
            Console.WriteLine("user id is" + userIdClaim);
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


        public async Task<bool> SendMessage(_Message message)
        {
         
            bool is_sent = false;
            var userIdClaim = Context.User?.FindFirst("user_id");
            if (userIdClaim == null)
            {
                Console.WriteLine("[ChatHub] Unauthorized message attempt");
                return false;
            }

            var senderId = userIdClaim.Value;
            MessageService _service = new MessageService(_msg_dl);
            message.sender_id = Convert.ToInt32(senderId);
            Console.WriteLine("message is sent by " + message.sender_id);


            if (message.type == "img")
            {
                Image_msg image_Msg = new Image_msg();
                image_Msg.type = message.type;
                image_Msg.sender_id = message.sender_id;
                image_Msg.reciever_id=message.reciever_id;
                image_Msg.image = message.img;
                image_Msg.time = message.time;
                image_Msg.caption=message.caption;
                image_Msg.is_seen = _chatConnections.ContainsKey(message.reciever_id.ToString());

                Console.WriteLine("image msg recieved from user " + message.sender_id);
                message.img_url = await _service.sendimgMessage(image_Msg);
                is_sent=!string.IsNullOrEmpty(message.img_url);



            }
            else if (message.type == "video")
            {
                Video_msg video_Msg = new Video_msg();
                video_Msg.type = message.type;
                video_Msg.sender_id = message.sender_id;
                video_Msg.reciever_id= message.reciever_id;
                video_Msg.time=message.time;
                video_Msg.caption=message.caption;
                video_Msg.duration=message.duration;
                video_Msg.is_seen= _chatConnections.ContainsKey(message.reciever_id.ToString());

                video_Msg.video = message.video;

                Console.WriteLine("a video received from user " + message.sender_id);
                message.video_url = await _service.sendvideoMessage(video_Msg);
                is_sent = !string.IsNullOrEmpty(message.video_url);

            }
            else if (message.type == "voice")
            {

                Audio_msg audio_Msg = new Audio_msg();
                audio_Msg.type = message.type;
                audio_Msg.time= message.time;
                audio_Msg.sender_id=message.sender_id;
                audio_Msg.reciever_id= message.reciever_id;
                audio_Msg.duration= message.duration;
                audio_Msg.voice = message.voice;
                audio_Msg.voice_byte = message.voice_byte;
                audio_Msg.file_name = message.file_name;
                audio_Msg.is_seen = _chatConnections.ContainsKey(message.reciever_id.ToString());


                Console.WriteLine("audio msg from user " + message.sender_id);
                message.voice_url = await _service.sendvoiceMessage(audio_Msg);
                is_sent= !string.IsNullOrEmpty(message.voice_url);


            }
            else if(message.type=="msg") 
            {
                Text_msg text_Msg = new Text_msg();
                text_Msg.type = message.type;
                text_Msg.sender_id= message.sender_id;
                text_Msg.time = message.time;
                text_Msg.reciever_id= message.reciever_id;
                text_Msg.text_msg = message.text_msg;
                text_Msg.is_seen = _chatConnections.ContainsKey(message.reciever_id.ToString());
                Console.WriteLine("text msg has " + text_Msg.sender_id.ToString() + text_Msg.text_msg+ " and time is "+text_Msg.time);
                is_sent = _service.sendtxtMessage(text_Msg);
            }




            if (is_sent)
            {
                // Send to recipient
                if (_chatConnections.TryGetValue(message.sender_id.ToString(), out var connections))
                {
                    foreach (var conn in connections)
                    {
                        var receiverMsg = new Message_DTO
                        {
                            sender_id = message.sender_id,
                            reciever_id = message.reciever_id,
                            type = message.type,
                            is_seen = _chatConnections.ContainsKey(message.reciever_id.ToString()),
                            text_msg = message.text_msg,
                            img_url = message.img_url,
                            video_url = message.video_url,
                            voice_url = message.voice_url,
                            caption = message.caption,
                            duration = message.duration,
                            time = message.time,
                            is_sent = true
                        };

                        await Clients.Client(conn).SendAsync("ReceiveMessage", receiverMsg);
                    }
                }

                // Also send to sender (for echo)
                if (_chatConnections.TryGetValue(message.reciever_id.ToString(), out var senderConnections))
                {
                    foreach (var conn in senderConnections)
                    {
                        var senderMsg = new Message_DTO
                        {
                            sender_id = message.sender_id,
                            reciever_id = message.reciever_id,
                            type = message.type,
                            is_seen = _chatConnections.ContainsKey(message.reciever_id.ToString()),
                            text_msg = message.text_msg,
                            img_url = message.img_url,
                            video_url = message.video_url,
                            voice_url = message.voice_url,
                            caption = message.caption,
                            duration = message.duration,
                            time = message.time,
                            is_sent = false
                        };

                        await Clients.Client(conn).SendAsync("ReceiveMessage", senderMsg);
                    }
                }

                Console.WriteLine($"Message sent: {message.text_msg}");
                return true;
            }
            else
            {
                Console.WriteLine("cannot send");
                return false;
            }

        }


    }
}
