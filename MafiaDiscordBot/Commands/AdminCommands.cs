using System.Threading.Tasks;
using MafiaDiscordBot.Discord;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using MafiaDiscordBot.Mafia.Defs;
using MafiaDiscordBot.Mafia.Game;
using MafiaDiscordBot.Mafia.Manager;

namespace MafiaDiscordBot.Commands;

public class AdminCommands(DefLoader defLoader, DiscordMessageManager _msgManager, SessionManager sessionManager) : ApplicationCommandModule<SlashCommandContext>
{
    // 디스코드에 등록될 슬래시 명령어 이름과 설명을 어트리뷰트([])로 지정합니다.
    [SlashCommand("데이터리로드", "기획 JSON 데이터를 다시 불러옵니다. (관리자 전용)")]
    public async Task ReloadDataAsync()
    {
        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        // 1. 기존 메모리에 있던 데이터를 싹 날립니다.
        
        DefDatabase<Def>.Clear();

        // 2. JSON 파일을 다시 읽어옵니다.
        defLoader.LoadAll();

        string title = "🔄 데이터 리로드 완료";
        string desc =
            $"현재 로드된 직업 수: **{DefDatabase<RoleDef>.Count}개**\n능력 수: **{DefDatabase<AbilityDef>.Count}**개\n 메세지 수: {DefDatabase<MessageDef>.Count}개";

        await _msgManager.ReplyEmbedAsync(Context, title, desc, new Color(0, 255, 0));
    }
    
    [SlashCommand("솔로테스트", "가상 봇 플레이어들을 소환하여 혼자서 게임을 테스트합니다.")]
    public async Task StartSoloTestAsync(int botCount = 3)
    {
        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

        ulong channelId = Context.Interaction.Channel.Id;

        // 1. 세션 가져오기 및 초기화
        var session = sessionManager.GetSession(channelId);
        session.Players.Clear(); // 기존 멤버 초기화

        // 2. 명령어 입력자(나)를 1번 플레이어로 등록
        session.Players.Add(new Player(Context.User.Id, Context.User.Username));

        // 3. 봇 투입 및 직업 배정
        sessionManager.AddVirtualPlayers(session, botCount);
        sessionManager.AssignRoles(session);

        // 4. 배정된 결과 문자열로 조립
        string desc = $"총 {session.Players.Count}명 참가 완료 (나 + 봇 {botCount}명)\n\n**[비밀 직업 배정표]**\n";
        foreach(var p in session.Players)
        {
            desc += $"- **{p.Name}** : {p.Role?.Label ?? "알 수 없음"}\n";
        }

        // 5. 나에게만 보이도록 결과 전송 (디버깅용)
        await _msgManager.ReplyEmbedAsync(Context, "🧪 솔로 테스트 환경 구축 완료", desc, new Color(0, 200, 255));
    }

    public async Task CheckDataAsync()
    {
        await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
    }
}