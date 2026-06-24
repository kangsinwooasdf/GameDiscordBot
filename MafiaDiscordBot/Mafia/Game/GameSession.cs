using MafiaDiscordBot.Mafia.Game.Event;

namespace MafiaDiscordBot.Mafia.Game;

public class GameSession
{
    public List<GameEvent> EventQueue { get; } = new();
}