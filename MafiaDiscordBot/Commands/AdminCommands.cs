using System.Threading.Tasks;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using MafiaDiscordBot.Mafia.Defs;

namespace MafiaDiscordBot.Commands;

public class AdminCommands(DefLoader defLoader) : ApplicationCommandModule<SlashCommandContext>
{
    // 디스코드에 등록될 슬래시 명령어 이름과 설명을 어트리뷰트([])로 지정합니다.
    [SlashCommand("데이터리로드", "기획 JSON 데이터를 다시 불러옵니다. (관리자 전용)")]
    public async Task<string> ReloadDataAsync()
    {
        // 1. 기존 메모리에 있던 데이터를 싹 날립니다.
        DefDatabase<RoleDef>.Clear();
        DefDatabase<AbilityDef>.Clear();

        // 2. JSON 파일을 다시 읽어옵니다.
        defLoader.LoadAll();

        // 3. 디스코드에 성공 메시지를 띄웁니다.
        return $"✅ 데이터 리로드 완료 (현재 직업 수: {DefDatabase<RoleDef>.Count}개, 능력 수: {DefDatabase<AbilityDef>.Count})";
    }
}