
using Darkorbit.Net.netty.handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Darkorbit.Helper.packets.requests;
using Darkorbit.Net.netty;

namespace Darkorbit.Helper.packets.handlers
{
    class DiscordChannelMessageHandler : Handler
    {
        public void Execute(HelperBrain brain, string[] packet)
        {
            var request = new DiscordChannelMessageRequest();
            request.Read(packet);

            //var globalRoom = Chat.Chat.StorageManager.Rooms[0];

            //ChatClient.SendToRoom("j%" + globalRoom.Id + "@" + request.Username.Replace("#", "|") + "@" + request.Content + "@" +
            //1 + "#", globalRoom);

            //Console.WriteLine(request.Username);
            //Console.WriteLine(request.Content);
        }
    }
}
