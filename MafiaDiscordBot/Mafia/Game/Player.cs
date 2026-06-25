using MafiaDiscordBot.Mafia.Defs;
using MafiaDiscordBot.Mafia.Game.Abilities;
using NetCord;

namespace MafiaDiscordBot.Mafia.Game;

public class Player
{
    // 유저 식별 정보
    public ulong UserID { get; }
    public string Name { get; }
    
    // 게임 데이터
    public RoleDef? Role { get; private set; }
    public List<IAbility> Abilities { get; } = new();
    
    // 인게임 상태
    public bool IsDead { get; private set; } = false;
    public HashSet<string> StatusTags { get; } = new();
    
    // 생성자
    public Player(ulong userid, string name)
    {
        UserID = userid;
        this.Name = name;
    }

    public void Die() =>  IsDead = true;
    public void ClearStatusTags() => StatusTags.Clear();
}