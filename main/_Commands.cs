using Discord.WebSocket;
using Discord_Bot_Temp.Classes;
using Discord_Bot_Temp.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Bot_Temp.main
{
    internal class _Commands
    {
        public static SocketMessage MessageSent;
        public static Command[] CommandList = new Command[]
        {
            new Command{Name = ".ping", Method = () => Program.Ping(MessageSent), Trigger = TriggerType.Is_Equal_To, definition = "Tells You what the bot stats are"},
            new Command{Name = ".help", Method = () => HELP.Help(MessageSent), Trigger = TriggerType.Starts_With, definition = "Tells You what all the commands are"},

        };
    }
}
