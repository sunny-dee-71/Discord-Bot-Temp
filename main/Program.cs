using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Discord_Bot_Temp.Classes;
using Discord_Bot_Temp.main;

class Program
{
    private static DiscordSocketClient _client;
    private static readonly HttpClient _httpClient = new HttpClient();

    static void Main(string[] args) 
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        new Program().RunBotAsync().GetAwaiter().GetResult(); 
    }

    public async Task RunBotAsync()
    {
        // Configure the client with only the necessary intents
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent | GatewayIntents.DirectMessages
        };



        _client = new DiscordSocketClient(config);

        _client.Log += Log;

        // Place your bot token here for local use
        string token = "MTMyNzM5NjE0MDI4NjQ3NjQwOA.GhI2Kk.UMuu9zN7DvAxdF6MPO7PKNd4t0xzBPdkRIkW0c";  // Your token here

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        _client.Ready += async () =>
        {
            Console.WriteLine("Bot is ready, registering slash commands...");
            await RegisterSlashCommands();  // Register slash commands after the bot is fully ready
        };

        _client.SlashCommandExecuted += HandleSlashCommandsAsync;


        _client.MessageReceived += MessageRecived;

        await Task.Delay(-1);
    }

    public async Task RegisterSlashCommands()
    {
        var restClient = _client.Rest;

        foreach (var command in _Commands.CommandList)
        {
            var commandBuilder = new SlashCommandBuilder()
                .WithName(command.Name.Replace(".", ""))  // Removing period for slash commands
                .WithDescription(command.definition);

            try
            {
                await restClient.CreateGlobalCommand(commandBuilder.Build());
                Console.WriteLine($"Registered slash command: {command.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering command {command.Name}: {ex.Message}");
            }
        }
    }







    private Task Log(LogMessage log)
    {
        Console.WriteLine(log);
        return Task.CompletedTask;
    }

    private async Task MessageRecived(SocketMessage message)
    {
        if (message.Author.IsBot) return;

        Console.WriteLine($"\n \n \n Message received: {message.Content}  ");
        Console.WriteLine($"Sender: {message.Author.Username}#{message.Author.Discriminator}");
        Console.WriteLine($"Sender ID: {message.Author.Id}");
        Console.WriteLine($"Channel: {message.Channel.Name}");

        if (message.Channel is SocketDMChannel)
        {
            // This message was sent in a DM
            await message.Channel.SendMessageAsync("Hello i am just a bot so i wont be abel to talk but you can still use my commands here.");
        }

        await HandleCommandsAsync(message);
        
    }

    public static async Task Ping(SocketMessage skibidi)
    {
        
        HandlePingCommand(skibidi);
    }

    public static async Task HandlePingCommand(SocketMessage message)
    {
        // Ensure the command is .ping
        if (message.Content.ToLower() != ".ping") return;

        // Get bot latency
        var latency = _client.Latency;

        // Get uptime
        var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;

        // Get memory usage
        var memoryUsage = Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024); // Convert bytes to MB

        // Get guild count and total channels
        var guildCount = _client.Guilds.Count;
        var channelCount = _client.Guilds.Sum(g => g.Channels.Count);

        // Get system info (optional)
        var os = RuntimeInformation.OSDescription;

        // Build response
        var response = $"**Bot Statistics:**\n" +
                       $"🏓 **Latency:** {latency}ms\n" +
                       $"⏱ **Uptime:** {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s\n" +
                       $"🛠 **Memory Usage:** {memoryUsage} MB\n" +
                       $"🌐 **Servers:** {guildCount}\n" +
                       $"💬 **Channels:** {channelCount}\n" +
                       $"🖥 **Operating System:** {os}";

        // Send the response
        await message.Channel.SendMessageAsync(response);
    }

    public async Task HandleCommandsAsync(SocketMessage message)
    {


        bool isDm = message.Channel is IDMChannel;

        bool sentByAdmin;
        bool isInNsfwChannel;

        if (isDm)
        {
            sentByAdmin = true;
            isInNsfwChannel = true;
        }
        else if (message.Author is SocketGuildUser user)
        {
            sentByAdmin = IsAdmin(user);

            if (message.Channel is SocketTextChannel textChannel)
            {
                isInNsfwChannel = textChannel.IsNsfw;
            }
            else
            {
                isInNsfwChannel = false;
            }
        }
        else
        {
            // Default fallback for unexpected scenarios
            sentByAdmin = false;
            isInNsfwChannel = false;
        }




        foreach (Command c in _Commands.CommandList)
        {
            if (c.admin && !sentByAdmin)
                continue;


            if (c.NSFW && !isInNsfwChannel)
                continue;


            string Content = message.Content;
            string CommandThing = c.Name;
            if (!c.Case_Sensitive)
            {
                CommandThing = c.Name.ToLower();
                Content = message.Content.ToLower();
            }

            if (c.Trigger == TriggerType.Contains)
            {
                if (Content.Contains(CommandThing))
                {
                    PreformCommand(c,message);
                }
            }

            if (c.Trigger == TriggerType.Starts_With)
            {
                if (Content.StartsWith(CommandThing))
                {
                    PreformCommand(c, message);
                }
            }

            if (c.Trigger == TriggerType.Is_Equal_To)
            {
                if (Content == CommandThing)
                {
                    PreformCommand(c, message);
                }
            }
        }
    }

    public bool IsAdmin(SocketGuildUser user)
    {
        // Check if the user is an administrator
        return user.GuildPermissions.Administrator;
    }

    private async Task PreformCommand(Command c,SocketMessage m)
    {
        _Commands.MessageSent = m;
        c.Method.Invoke();
    }


    public async Task HandleSlashCommandsAsync(SocketSlashCommand slashCommand)
    {
        // Same checks for DM, admin, and NSFW channel (you can adapt this for your needs)
        bool isDm = slashCommand.Channel is IDMChannel;

        bool sentByAdmin;
        bool isInNsfwChannel;

        if (isDm)
        {
            sentByAdmin = true;
            isInNsfwChannel = true;
        }
        else if (slashCommand.User is SocketGuildUser user)
        {
            sentByAdmin = IsAdmin(user);

            if (slashCommand.Channel is SocketTextChannel textChannel)
            {
                isInNsfwChannel = textChannel.IsNsfw;
            }
            else
            {
                isInNsfwChannel = false;
            }
        }
        else
        {
            sentByAdmin = false;
            isInNsfwChannel = false;
        }

        // Iterate through the list of commands and process triggers (same logic as HandleCommandsAsync)
        foreach (Command c in _Commands.CommandList)
        {
            // Check for admin, NSFW conditions
            if (c.admin && !sentByAdmin)
                continue;

            if (c.NSFW && !isInNsfwChannel)
                continue;

            // For slash commands, we can use the 'Name' directly
            string CommandThing = c.Name;

            // If the trigger is case-insensitive
            string content = slashCommand.Data.Name;
            if (!c.Case_Sensitive)
            {
                CommandThing = c.Name.ToLower();
                content = content.ToLower();
            }

            // Check the trigger type and execute if matched
            if (c.Trigger == TriggerType.Contains && content.Contains(CommandThing))
            {
                PerformCommand(c, slashCommand);
            }
            else if (c.Trigger == TriggerType.Starts_With && content.StartsWith(CommandThing))
            {
                PerformCommand(c, slashCommand);
            }
            else if (c.Trigger == TriggerType.Is_Equal_To && content == CommandThing)
            {
                PerformCommand(c, slashCommand);
            }
        }
    }


    // Perform command execution (this part remains the same, invoking the method)
    private async Task PerformCommand(Command c, SocketSlashCommand slashCommand)
    {
        _Commands.MessageSent = null;  // You might not need MessageSent here, but you can still use it for any necessary context

        // Invoke the method associated with the command
        c.Method.Invoke();

        // Respond to the slash command (optional)
        await slashCommand.RespondAsync($"Executed command: {c.Name}");
    }





}
