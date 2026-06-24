using MafiaDiscordBot.Mafia.Core;
using MafiaDiscordBot.Mafia.Game;

namespace MafiaDiscordBot.Mafia.Manager;

public class GameEventManager
{
    public NightReport ProcessQueue(GameSession session)
    {
        // 1. 텅 빈 보고서(종이)를 하나 새로 꺼냅니다.
        var report = new NightReport();

        // 2. 큐에 쌓인 이벤트들을 우선순위(Priority) 오름차순으로 정렬합니다.
        // (숫자가 낮을수록 먼저 실행됨. 예: 의사 300 -> 마피아 500)
        var sortedEvents = session.EventQueue.OrderBy(e => e.Priority).ToList();

        // 3. 순서대로 실행(Resolve)하면서, 빈 보고서를 넘겨주어 결과를 적게 합니다.
        foreach (var gameEvent in sortedEvents)
        {
            // 앞서 만든 AttackEvent.Resolve 내부에서 이 report.DeadPlayers에 사망자를 추가하게 됩니다.
            gameEvent.Resolve(session, report);
        }

        // 4. 하룻밤 정산이 끝났으니 큐를 깨끗하게 비웁니다.
        session.EventQueue.Clear();

        // 5. 내용이 꽉 채워진 보고서를 반환합니다.
        return report;
    }
}