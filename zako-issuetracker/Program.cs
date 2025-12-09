using Discord;
//using Discord.Interactions.Builders;
using Discord.Commands;
using Discord.WebSocket;

namespace zako_issuetracker;

public enum IssueTag
{
    Bug, Feature, Enhancement
}

public enum IssueStatus
{
    Proposed, Approved, Rejected, InProgress, Completed
}
class Program
{
    private static DiscordSocketClient _client;

    public static async Task Main(string[] args)
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.All
            
        };
        
        _client = new DiscordSocketClient(config);
        
        _client.Log += LogAsync;
        _client.Ready += ReadyAsync;
        _client.MessageReceived += MessageReceivedAsync;
        _client.InteractionCreated += InteractionCreatedAsync;
     
        await _client.LoginAsync(TokenType.Bot, "MTQ0NzE2Nzc2MDU3MTMwNjAzNQ.GfwkL8.t0FgeFgQdAZEqaHyyU7tFaEHyYlOTHdiaBuSAU"); // token here
        
        await _client.StartAsync();
        
        await Task.Delay(Timeout.Infinite);
    }
    
    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }
    
    private static async Task ReadyAsync()
    {
        Console.WriteLine($"{_client.CurrentUser} is connected!");

        var guild = _client.GetGuild(1429043368876314696);

        _client.SetActivityAsync(new Game("Gathering Issues..."));

        var issueTagChoices = new SlashCommandOptionBuilder()
            .WithName("tags")
            .WithDescription("Tags of issue")
            .WithRequired(true)
            .WithType(ApplicationCommandOptionType.String);
        foreach (var tag in Enum.GetNames(typeof(IssueTag)))
        {
            issueTagChoices.AddChoice(tag, tag.ToLowerInvariant());
        }

        var issueStatusChoies = new SlashCommandOptionBuilder()
            .WithName("change-to")
            .WithDescription("Change issue status to")
            .WithRequired(true)
            .WithType(ApplicationCommandOptionType.String);

        foreach (var status in Enum.GetNames(typeof(IssueStatus)))
        {
            issueStatusChoies.AddChoice(status, status.ToLowerInvariant());
        }
        
        var newIssue = new SlashCommandBuilder()
            .WithName("issue")
            .WithDescription("Root of issue commands")
            //.AddOptions(new SlashCommandOptionBuilder())
            .WithNameLocalizations(new Dictionary<string, string>
            {
                {"ko","이슈"}
            })
            .WithDescriptionLocalizations(new Dictionary<string, string>
            {
                {"ko","이슈 명령어 최상위 식별자(?)"}
            })
            //new
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("new")
                .WithDescription("Create new issue")
                .WithType(ApplicationCommandOptionType.SubCommand))
            // status
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("status")
                .WithDescription("set status")
                .AddOption(new SlashCommandOptionBuilder() // status(int issueId)
                    .WithName("id")
                    .WithDescription("Issue ID")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Integer))
                .AddOption(issueStatusChoies)
                .WithType(ApplicationCommandOptionType.SubCommand))
            // list
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("list")
                .WithDescription("List of \"issues\"")
                .AddOption(issueTagChoices) // list (<enum> Tag tag)
                .AddOption(new SlashCommandOptionBuilder() // list (bool opened = true)
                    .WithName("opened")
                    .WithDescription("True = show opened issue only, false = show all")
                    //.WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Boolean))
                .WithType(ApplicationCommandOptionType.SubCommand))
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("export")
                .WithDescription("Export the issue")
                .AddOption(issueTagChoices)
                .WithType(ApplicationCommandOptionType.SubCommand))
            .Build();
        
        await guild.CreateApplicationCommandAsync(newIssue);
        //return Task.CompletedTask;
    }
    
    private static async Task MessageReceivedAsync(SocketMessage message)
    {
        // The bot should never respond to itself.
        if (message.Author.Id == _client.CurrentUser.Id)
            return;


        if (message.Content == "!issue")
        {
            var cb = new ComponentBuilder()
                .WithButton("Click me!", "unique-id", ButtonStyle.Primary);

            // Send a message with content 'pong', including a button.
            // This button needs to be build by calling .Build() before being passed into the call.
            await message.Channel.SendMessageAsync("pong!", components: cb.Build());
        }
    }
    private static async Task InteractionCreatedAsync(SocketInteraction interaction)
    {
        // safety-casting is the best way to prevent something being cast from being null.
        // If this check does not pass, it could not be cast to said type.
        if (interaction is SocketMessageComponent component)
        {
            // Check for the ID created in the button mentioned above.
            if (component.Data.CustomId == "unique-id")
                await interaction.RespondAsync("Thank you for clicking my button!");

            else
                Console.WriteLine("An ID has been received that has no handler!");
        }

        if (interaction is SocketModal modal)
        {
            switch (modal.Data.CustomId)
            {
                case "ISSUE_MODAL":
                {
                    var s = modal.Data.Components.First(
                        x => x.CustomId == "issue_title")?.Value ?? "-";

                    var c = modal.Data.Components.ToArray();
                    object[] values = new object[c.Length];
                    for (int i = 0; i < c.Length; i++)
                        values[i] = c[i].Value ?? "NULL";

                    values[1] = c[1].Values.First();
                    Console.WriteLine($"values[1] = {values[1].ToString()}");
                    Console.WriteLine();
                    
                    
                    var embed = new EmbedBuilder().WithTitle("새 이슈가 생성되었어요!")
                        .AddField("Issue Name", values[0])
                        .AddField("Issue Tag", values[1])
                        .AddField("Issue Detail", values[2])
                        
                        //.AddField("이슈 이름", modal.Data.Components.First(x => x.CustomId == "issue_title")?.Value)
                        //.AddField("이슈 태그", modal.Data.Components.First(x => x.CustomId == "issue_tag")?.Value)
                        //.AddField("이슈 설명", modal.Data.Components.First(x => x.CustomId == "issue_detail")?.Value)
                        .WithColor(Color.Blue)
                        .WithCurrentTimestamp()
                        .Build();

                    await modal.RespondAsync(embed: embed, ephemeral:true);
                }
                    break;
                default:
                    await modal.RespondAsync("undfined command");
                    break;
            }
        }

        if (interaction is SocketSlashCommand slashCommand)
        {
            switch (slashCommand.Data.Name)
            {
                case "issue":
                    var subCommand = slashCommand.Data.Options.First().Name;
                    switch (subCommand)
                    {
                        case "new":
                            
                            var inModal = new ModalBuilder()
                                .WithTitle("새 이슈")
                                .WithCustomId("ISSUE_MODAL")
                                .AddTextInput("이슈 이름", "issue_title", placeholder:"이슈 이름을 입력하세요", required:true)
                                //.AddComponents(new List<IMessageComponent>{tagSel}, 1)
                                .AddSelectMenu("이슈 태그", "issue_tag", options:new List<SelectMenuOptionBuilder>
                                {
                                    new SelectMenuOptionBuilder().WithLabel("Bug").WithValue("bug").WithDescription("오류가 발생했어요!")
                                    ,new SelectMenuOptionBuilder().WithLabel("Feature").WithValue("feature").WithDescription("새로운 기능을 제안해요!")
                                    ,new SelectMenuOptionBuilder().WithLabel("Enhancement").WithValue("enhancement").WithDescription("기존 기능을 개선해요!")
                                }, required:true)
                                .AddTextInput("이슈 설명", "issue_detail", placeholder:"이슈 설명을 입력하세요", required:true, style: TextInputStyle.Paragraph);
                                
                            await slashCommand.RespondWithModalAsync(inModal.Build());
                            return;
                        case "status":
                        {
                            
                        }
                            return;
                        case "list":
                            return;
                        case "export":
                            return;
                        default:
                            await slashCommand.RespondAsync("Unknown command");
                            break;
                    }
                    break;
                default:
                    await slashCommand.RespondAsync("Unkown command");
                    break;
            }
        }
    }
}