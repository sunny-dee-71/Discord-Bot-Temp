using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class GamesModule : ModuleBase<SocketCommandContext>
{

    public static void rps(SocketMessage message)
    {
        PlayRPS(message);
    }

    private static async Task PlayRPS(SocketMessage message)
    {
        var options = new[] { "rock", "paper", "scissors" };
        var random = new Random();
        var botChoice = options[random.Next(options.Length)];
        var userMessage = message.Content.ToLower().Replace(".rps", "").Trim();

        // Validate the user's choice
        if (!options.Contains(userMessage))
        {
            await message.Channel.SendMessageAsync("Invalid choice! Please choose `rock`, `paper`, or `scissors`. Example: `.rps rock`");
            return;
        }

        // Determine the result
        string result;
        if (userMessage == botChoice)
        {
            result = "It's a tie! 🤝";
        }
        else if ((userMessage == "rock" && botChoice == "scissors") ||
                 (userMessage == "paper" && botChoice == "rock") ||
                 (userMessage == "scissors" && botChoice == "paper"))
        {
            result = "You win! 🎉";
        }
        else
        {
            result = "You lose! 😢";
        }

        // Build the response
        var response = $"**Rock, Paper, Scissors!**\n" +
                       $"🧑 **Your Choice:** {userMessage}\n" +
                       $"🤖 **Bot's Choice:** {botChoice}\n" +
                       $"🏆 **Result:** {result}";

        // Send the response
        await message.Channel.SendMessageAsync(response);
    }

}
