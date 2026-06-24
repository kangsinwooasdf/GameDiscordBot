namespace MafiaDiscordBot.Mafia.Game.Abilities;

public interface IAbility
{
    public void Use(GameSession session, Player caster, Player target);
}