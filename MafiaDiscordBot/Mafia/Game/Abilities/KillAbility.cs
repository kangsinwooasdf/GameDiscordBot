using MafiaDiscordBot.Mafia.Defs;
using MafiaDiscordBot.Mafia.Game.Event;

namespace MafiaDiscordBot.Mafia.Game.Abilities;

public class KillAbility(AbilityDef def) : IAbility
{
    public AbilityDef Def { get; } = def;

    public void Use(GameSession session, Player caster, Player target)
    {
        var attackEvent = new AttackEvent(caster, target, Def.BasePriority);
        session.EventQueue.Add(attackEvent);
    }
}