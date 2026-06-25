using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using MafiaDiscordBot.Debug;
using Microsoft.Extensions.Logging;

namespace MafiaDiscordBot.Mafia.Defs;

public class DefLoader(ILogger<DefLoader> logger)
{
    public void LoadAll()
    {
        // Log.Info("모든 Def 데이터를 로드합니다...", LogMessageType.System);
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
        
        jsonOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());


        // 1. 리플렉션을 위해 LoadDirectory<T> 메서드와 DefDatabase<T> 타입을 가져옵니다.
        var loadMethod = typeof(DefLoader).GetMethod("LoadDirectory", BindingFlags.NonPublic | BindingFlags.Instance);
        var clearMethodBase = typeof(DefDatabase<>);

        // 2. 등록된 모든 Def 종류를 foreach로 순회.
        foreach (var (defType, folderName) in DefDatabase.DefRegistry)
        {
            string fullFolderPath = Path.Combine(baseDataPath, folderName);

            // 기존 데이터를 비우는 DefDatabase<T>.Clear() 호출
            var dbType = clearMethodBase.MakeGenericType(defType);
            dbType.GetMethod("Clear", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);

            // LoadDirectory<T>(folderName) 호출
            object[] parameters = [fullFolderPath, jsonOptions];
            loadMethod?.MakeGenericMethod(defType).Invoke(this, parameters);
        }

        logger.LogInformation("데이터 로딩 완료 (직업: {RoleCount}개, 능력: {AbilityCount}개, 메세지: {MessageCount}개)", 
            DefDatabase<RoleDef>.Count, DefDatabase<AbilityDef>.Count, DefDatabase<MessageDef>.Count);
        // Log.Success("✅ 모든 Def 로드 완료!", LogMessageType.System);
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

                if (def == null) continue;
                DefDatabase<T>.Add(def);
                
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("로드 성공: {DefName}", def.DefName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "파일 로드 실패: {FileName}", Path.GetFileName(file));
            }
        }
    }
}