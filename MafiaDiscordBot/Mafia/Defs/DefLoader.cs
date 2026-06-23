using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MafiaDiscordBot.Mafia.Defs;

public class DefLoader(ILogger<DefLoader> logger)
{
    private readonly ILogger<DefLoader> _logger;
    
    public void LoadAll(ILogger logger)
    {
        // 실행 파일(.exe)이 있는 폴더 내부의 Data 폴더를 찾습니다.
        string baseDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

        if (!Directory.Exists(baseDataPath))
        {
            logger.LogWarning("데이터 폴더를 찾을 수 없습니다: {Path}", baseDataPath);
            return;
        }

        var jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true, // json의 낙타표기법과 C#의 파스칼표기법 자동 매핑
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        // Data/Jobs 폴더 안의 모든 json을 읽어 RoleDef로 파싱합니다.
        LoadDirectory<RoleDef>(Path.Combine(baseDataPath, "Jobs"), jsonOptions);
            
        _logger.LogInformation("데이터 로딩 완료! (직업: {RoleCount}개)", DefDatabase<RoleDef>.Count);
    }

    private void LoadDirectory<T>(string targetDirectory, JsonSerializerOptions options) where T : Def
    {
        if (!Directory.Exists(targetDirectory)) return;

        var files = Directory.GetFiles(targetDirectory, "*.json", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            try
            {
                string jsonString = File.ReadAllText(file);
                T? def = JsonSerializer.Deserialize<T>(jsonString, options);

                if (def != null)
                {
                    DefDatabase<T>.Add(def);
                    _logger.LogDebug("로드 성공: {DefName}", def.DefName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 로드 실패: {FileName}", Path.GetFileName(file));
            }
        }
    }
}