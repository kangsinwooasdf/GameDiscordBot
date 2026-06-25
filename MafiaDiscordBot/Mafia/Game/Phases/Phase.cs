using Mafia;
using MafiaDiscordBot.Mafia.Game;

namespace MafiaDiscordBot.Mafia;

public abstract class Phase : IGamePhase
{
    public abstract string Name { get; }
    public virtual Task EnterAsync(GameSession session)
    {
        return Task.CompletedTask;    
    }

    public virtual Task UpdateAsync(GameSession session)
    {
        return Task.CompletedTask;
    }

    public virtual Task ExitAsync(GameSession session)
    {
        return Task.CompletedTask;
    }
}