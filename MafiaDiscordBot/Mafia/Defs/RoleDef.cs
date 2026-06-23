namespace MafiaDiscordBot.Mafia.Defs;

public class RoleDef : Def
{
    // 이 직업이 기본적으로 가지는 능력들의 ID 리스트
    public List<string> BaseAbilities { get; set; } = new();

    public string Team { get; set; } = string.Empty;
}