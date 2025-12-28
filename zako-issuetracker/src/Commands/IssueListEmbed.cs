using Discord;

namespace zako_issuetracker.commands;

public static class IssueListEmbed
{
    private static int PageSize = EnvLoader.GetPageSize();
    
    
    public static Embed[] BuildIssueListEmbed(Dictionary<int, Issue.IssueContent> dict, int page, IssueTag? tag = null, IssueStatus? status = null)
    {
        string sTag = tag?.ToString() ?? "All";
        string sStatus = status?.ToString() ?? "All";
        
        var eb = new List<EmbedBuilder>();
        
        
        eb[0].WithTitle($"Issue List :: Page {page}");
        eb[0].WithDescription($"tag : {sTag} | status : {sStatus}");
        eb[0].WithColor(Color.Blue);
        eb[5].WithFooter($"Page {page} | Total Issues: {dict.Count}");
        

        var appearedIssues = dict.Skip((page -1) * PageSize).Take(PageSize);
        foreach (var ctx in appearedIssues)
        {
            eb[ctx.Key + 1].AddField($"ID : {ctx.Key.ToString()}",
                $"Name : {ctx.Value.Name}\n" +
                $"Detail : {ctx.Value.Detail}\n"+
                $"Tag : {ctx.Value.Tag.ToString()}\n"+
                $"Status : {ctx.Value.Status.ToString()}\n"+
                $"User : <@{ctx.Value.UserId}>");
            eb[ctx.Key + 1].WithTimestamp(DateTimeOffset.Now);
        }
        return eb.Select(b => b.Build()).ToArray();
    }
}