using MafiaDiscordBot.Mafia.Core;
using MafiaDiscordBot.Mafia.Defs;
using MafiaDiscordBot.Mafia.Manager;

namespace MafiaDiscordBot.Mafia.Game.Phases;

public class DawnPhase(GameEventManager eventManager) : Phase
{
    public override string Name => "새벽";
    
    private readonly GameEventManager _eventManager = eventManager;

    public override Task EnterAsync(GameSession session)
    {
        NightReport report = _eventManager.ProcessQueue(session);

        if (report.DeadPlayers.Any())
        {
            string deadName = string.Join(", ", report.DeadPlayers.Select(p => p.Name));

            var msgDef = DefDatabase<MessageDef>.Get("Msg_DawnDeathReport");
            string finalMessage = msgDef.FormatContent(deadName);
            
            // await
        }
        return null;
    }
}