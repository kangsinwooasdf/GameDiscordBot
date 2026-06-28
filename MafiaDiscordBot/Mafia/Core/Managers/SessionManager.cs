using System.Collections.Concurrent;
using MafiaDiscordBot.Mafia.Core;
using MafiaDiscordBot.Mafia.Defs;
using MafiaDiscordBot.Mafia.Game;

namespace MafiaDiscordBot.Mafia.Manager;

public class SessionManager
{
    // 디코 로비 생성, 보관, 삭제, 실행 담당
    private readonly ConcurrentDictionary<ulong, GameSession> _sessions = new();

    // 로비 생성 메소드
    public GameSession? CreateLobby(ulong channelId)
    {
        // 이미 해당 채널에 진행 중인 게임이 있는지 확인
        if (_sessions.ContainsKey(channelId))
        {
            // Log.Warning($"[SessionManager] 이미 채널({channelId})에 활성화된 게임 로비가 있습니다.");
            return null;
        }

        var newSession = new GameSession();
        // 추후 GameSession 클래스에 ChannelId 프로퍼티가 추가된다면 여기서 주입해줍니다.
        // newSession.ChannelId = channelId; 

        // 딕셔너리에 안전하게 추가 시도
        if (_sessions.TryAdd(channelId, newSession))
        {
            // Log.Info($"[SessionManager] 채널({channelId})에 새로운 게임 로비가 생성되었습니다.", LogMessageType.System);
            return newSession;
        }

        return null;
    }
    
    public bool RemoveLobby(ulong channelId)
    {
        // 딕셔너리에서 세션 제거 시도
        if (_sessions.TryRemove(channelId, out var removedSession))
        {
            // 필요하다면 여기서 removedSession 내부의 자원(타이머 등)을 해제(Dispose)할 수 있습니다.
            // Log.Info($"[SessionManager] 채널({channelId})의 게임 로비가 정상적으로 삭제되었습니다.", LogMessageType.System);
            return true;
        }

        // Log.Warning($"[SessionManager] 삭제할 로비를 찾을 수 없습니다. (Channel: {channelId})");
        return false;
    }
    
    public GameSession GetSession(ulong channelId)
    {
        if (!_sessions.TryGetValue(channelId, out var session))
        {
            session = new GameSession();
            _sessions[channelId] = session;
        }

        return session;
    }

    // 가상 플레이어 투입 메소드
    public void AddVirtualPlayers(GameSession session, int botCount)
    {
        for (int i = 1; i <= botCount; i++)
        {
            ulong dummyId = (ulong)(10000 + i);
            session.Players.Add(new Player(dummyId, $"bot_{i}"));
        }
        // todo Console.WirteLine 구문 싹다 Log 클래스 구현해서 변경
        Console.WriteLine($"디버그: 가상 플레이어 {botCount}명 투입 성공");
    }

    public void AssignRoles(GameSession session)
    {
        var players = session.Players;
        var allRoles = DefDatabase<RoleDef>.AllDefs.ToList();

        if (allRoles.Count == 0)
        {
            Console.WriteLine("[Error] 로드된 직업(RoleDef)이 없습니다! 데이터를 확인하세요.");
            return;
        }

        var random = new Random();
        var availableRoles = new List<RoleDef>();

        // (임시 테스트용) 필수 직업 1개씩 우선 풀(Pool)에 넣기
        var mafiaDef = DefDatabase<RoleDef>.Get("Role_Mafia");
        var policeDef = DefDatabase<RoleDef>.Get("Role_Police");
        var doctorDef = DefDatabase<RoleDef>.Get("Role_Doctor");

        if (mafiaDef != null) availableRoles.Add(mafiaDef);
        if (policeDef != null && players.Count >= 4) availableRoles.Add(policeDef);
        if (doctorDef != null && players.Count >= 5) availableRoles.Add(doctorDef);

        // 남은 인원수만큼 랜덤으로 직업 채우기 (추후 시민 등으로 고정 가능)
        while (availableRoles.Count < players.Count)
        {
            availableRoles.Add(allRoles[random.Next(allRoles.Count)]);
        }

        // 플레이어 목록 무작위 섞기 (Fisher-Yates 알고리즘 대체용 간단 셔플)
        var shuffledPlayers = players.OrderBy(x => random.Next()).ToList();

        // 분배 및 팩토리 조립!
        for (int i = 0; i < shuffledPlayers.Count; i++)
        {
            var player = shuffledPlayers[i];
            var role = availableRoles[i];

            // AbilityFactory가 JSON 문자열 배열을 실제 C# 능력 객체로 조립해줍니다!
            var abilities = AbilityFactory.CreateAbilityList(role.BaseAbilities);
                
            player.AssignRole(role, abilities);

            Console.WriteLine($"[Debug] 배정 완료: {player.Name} -> {role.Label} (능력 {abilities.Count}개)");
        }
    }
}