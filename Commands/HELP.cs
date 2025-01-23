using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord_Bot_Temp.main;

namespace Discord_Bot_Temp.Commands
{
    internal class HELP
    {

        public static async Task Help(SocketMessage message)
        {
            Console.WriteLine(message.ToString() + " has beem recived");
            var args = message.Content.Split(' ');
            var command = args.Length > 1 ? args[1] : null;
            await HandleHelpCommand(message, command);
        }


        private static async Task HandleHelpCommand(SocketMessage message, string command = null)
        {
            var messageReference = new MessageReference(message.Id);

            if (string.IsNullOrWhiteSpace(command))
            {
                // Display the list of all commands
                var helpMessage = new StringBuilder("**Here are the available commands:**\n");

                foreach (var cmd in _Commands.CommandList)
                {
                    helpMessage.AppendLine($"- **{cmd.Name}**: {cmd.definition} (Trigger: {cmd.Trigger})");
                }

                helpMessage.AppendLine("\nUse `.help <command>` to get more details about a specific command.");
                await message.Channel.SendMessageAsync(helpMessage.ToString(), messageReference: messageReference);
            }
            else
            {
                // Search for the specific command
                var matchedCommand = _Commands.CommandList.FirstOrDefault(c =>
                    c.Name.Equals(command, StringComparison.OrdinalIgnoreCase));

                if (matchedCommand != null)
                {
                    // Build and display details for the matched command
                    var helpMessage = new StringBuilder()
                        .AppendLine($"**Command:** {matchedCommand.Name}")
                        .AppendLine($"**Description:** {matchedCommand.definition}")
                        .AppendLine($"**Admin Only:** {(matchedCommand.admin ? "Yes" : "No")}")
                        .AppendLine($"**NSFW:** {(matchedCommand.NSFW ? "Yes" : "No")}")
                        .AppendLine($"**Case Sensitive:** {(matchedCommand.Case_Sensitive ? "Yes" : "No")}")
                        .AppendLine($"**Trigger Type:** {matchedCommand.Trigger}");

                    await message.Channel.SendMessageAsync(helpMessage.ToString(), messageReference: messageReference);
                }
                else
                {
                    // Notify if the command was not found
                    await message.Channel.SendMessageAsync(
                        $"Command `{command}` not found. Use `.help` to see the list of available commands.",
                        messageReference: messageReference
                    );
                }
            }
        }





    }
}
