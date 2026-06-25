using MafiaDiscordBot.Mafia;
using MafiaDiscordBot.Mafia.Game;

namespace Mafia;

public interface IGamePhase
{ 
// 페이즈의 이름 (예: "밤", "새벽", "낮 투표")
    string Name { get; }

    // 페이즈가 시작될 때 1회 호출됨 (메시지 전송, UI 생성 등)
    Task EnterAsync(GameSession session);

    // 페이즈 진행 중 주기적으로 또는 특정 이벤트 시 호출됨 (투표 완료 체크, 타이머 체크 등)
    Task UpdateAsync(GameSession session);

    // 페이즈가 종료되고 다음 페이즈로 넘어갈 때 1회 호출됨 (UI 비활성화, 타이머 종료 등)
    Task ExitAsync(GameSession session);
}
