using MafiaDiscordBot.Mafia;

namespace Mafia;

public interface IGamePhase
{ 
    Phase CurrentlyPhase { get; }
    void EnterPhase(Phase phase);
    void ExecutePhase();
    void ExitPhase();
}
