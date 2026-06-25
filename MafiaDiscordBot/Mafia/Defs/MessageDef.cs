namespace MafiaDiscordBot.Mafia.Defs;

public class MessageDef : Def
{
    public string Content { get; set; } =  string.Empty;
    public string PlaceHolder { get; set; } =  string.Empty;
    
    public string FormatContent(params object[] args)
    {
        return string.Format(Content, args);
    }
}