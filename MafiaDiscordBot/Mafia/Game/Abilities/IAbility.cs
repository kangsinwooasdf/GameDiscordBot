using MafiaDiscordBot.Mafia.Defs;

namespace MafiaDiscordBot.Mafia.Game.Abilities;

public interface IAbility
{
    AbilityDef Def { get; } // 정의 파일 데이터

    public void Use(GameSession session, Player caster, Player target);
}