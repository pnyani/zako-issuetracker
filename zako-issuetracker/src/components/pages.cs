using Discord;

namespace zako_issuetracker.components;

public static class Pages
{
    public static ComponentBuilder Button()
    {
        var buttons = new ComponentBuilder()
            .WithButton(label: "Previous", customId: "issue-previous", style: ButtonStyle.Primary, new Emoji("⬅️"))
            .WithButton(label: "Next", customId: "issue-next", style: ButtonStyle.Secondary, new Emoji("➡️"));

        return buttons;
    }
    
    
}