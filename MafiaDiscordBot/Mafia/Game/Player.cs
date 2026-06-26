using MafiaDiscordBot.Mafia.Defs;
using MafiaDiscordBot.Mafia.Game.Abilities;
using NetCord;

namespace MafiaDiscordBot.Mafia.Game;

public class Player(ulong userid, string name)
{
    // 유저 식별 정보
    public ulong UserID { get; } = userid;
    public string Name { get; } = name;

    // 게임 데이터
    public RoleDef? Role { get; private set; }
    public List<IAbility> Abilities { get; } = new();
    
    // 인게임 상태
    public bool IsDead { get; private set; } = false;
    public HashSet<string> StatusTags { get; } = [];

    public void AssignRole(RoleDef role, IEnumerable<IAbility> abilities)
    {
        Role = role;
        Abilities.Clear();
        Abilities.AddRange(abilities);
    }
    
    public void Die() =>  IsDead = true;
    public void ClearStatusTags() => StatusTags.Clear();
}