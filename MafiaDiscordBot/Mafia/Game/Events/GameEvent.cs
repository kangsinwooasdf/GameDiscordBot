using MafiaDiscordBot.Mafia.Core;

namespace MafiaDiscordBot.Mafia.Game.Event;

public abstract record GameEvent(int Priority)
{
    // 의사의 힐 등에 의해 이 행동이 무효화되었는지 체크
    public bool IsCancelled { get; set; } = false;
        
    // 이벤트를 정산할 때 호출될 메서드
    public abstract void Resolve(GameSession session, NightReport report);
}

public record AttackEvent(Player Caster, Player Target, int Priority) :  GameEvent(Priority)
{
    public override void Resolve(GameSession session, NightReport report)
    {
        // 누군가(의사 등)에 의해 이미 취소되었다면 아무 일도 일어나지 않음
        if (IsCancelled) return;

        // 취소되지 않았다면 타겟을 죽이고 보고서에 이름 등록
        Target.Die();
        report.DeadPlayers.Add(Target);
    }
}

public record HealEvent(Player Caster, Player Target, int Priority) : GameEvent(Priority)
{
    public override void Resolve(GameSession session, NightReport report)
    {
        if (IsCancelled) return;

        // 이벤트 큐를 뒤져서, '나와 타겟이 같은' AttackEvent를 찾아냅니다.
        // (의사가 우선순위가 더 높으므로, AttackEvent는 아직 실행 전 상태로 큐에 대기 중입니다)
        var attackEvent = session.EventQueue.OfType<AttackEvent>().FirstOrDefault(e => e.Target == Target);

        // 만약 타겟을 노리는 공격이 있었다면? -> 공격을 취소시키고 힐 성공 보고!
        if (attackEvent != null)
        {
            attackEvent.IsCancelled = true;
            report.WasHealSuccessful = true;
        }
    }
}