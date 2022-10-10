
using Darkorbit.Helper.packets;
using Darkorbit.Helper.packets.commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Darkorbit.Helper
{
    class HelperBrain
    {
        /// <summary>
        /// Instance -> Generated after connection is established
        /// </summary>
        public static HelperBrain _instance;

        /// <summary>
        /// Handler -> auto generated
        /// </summary>
        public static Handler Handler = new Handler();

        /// <summary>
        /// Client which is going to be for sending packets
        /// </summary>
 

        /// <summary>
        /// Discord ID: Used for identifying
        /// </summary>
        public string DiscordId { get; set; }

        /// <summary>
        /// Used in chat or other places
        /// </summary>
        public string DiscordName { get; set; }

        
    }
}
