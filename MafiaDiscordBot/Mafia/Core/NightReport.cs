using MafiaDiscordBot.Mafia.Game;

namespace MafiaDiscordBot.Mafia.Core;

public class NightReport
{
    public List<Player> DeadPlayers { get; } = new();
        
    // 의사의 힐이 성공했는지 여부
    public bool WasHealSuccessful { get; set; } = false;
}