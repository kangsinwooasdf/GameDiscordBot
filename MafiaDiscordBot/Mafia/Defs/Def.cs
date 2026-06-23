namespace MafiaDiscordBot.Mafia.Defs;

public abstract class Def
{
    // 시스템 식별자
    public string DefName { get; set; } = string.Empty;
    
    // 상속 시스템
    // 이 데이터를 복사해올 부모의 이름
    public string? ParentName { get; set; }
    public bool IsAbstract { get; set; } = false;
    
    // UI
    public string Label { get; set; } = string.Empty;
    public string DefDescription{ get; set; } = string.Empty;
}