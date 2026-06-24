using MafiaDiscordBot.Mafia;

namespace Mafia;
public class GameStateManager : IGamePhase
{
    public Phase CurrentlyPhase { get; }

    public void EnterPhase(Phase phase)
    {
        throw new NotImplementedException();
    }

    public void ExecutePhase()
    {
        throw new NotImplementedException();
    }

    public void ExitPhase()
    {
        throw new NotImplementedException();
    }
}