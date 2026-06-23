using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace MafiaDiscordBot.Mafia.Defs;

public class DefLoader(ILogger<DefLoader> logger)
{
    public void LoadAll()
        {
            // 실행 파일이 있는 위치(bin/Debug/...) 안의 Data 폴더를 기준으로 탐색
            string baseDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (!Directory.Exists(baseDataPath))
            {
                logger.LogWarning("데이터 폴더를 찾을 수 없습니다: {Path}", baseDataPath);
                return;
            }

            // JSON의 camelCase(defName)와 C#의 PascalCase(DefName)를 자동 매핑
            var jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            // 1. 직업 데이터 로드 (Data/Jobs 폴더)
            LoadDirectory<RoleDef>(Path.Combine(baseDataPath, "Jobs"), jsonOptions);
            
            // 2. 능력 데이터 로드 (Data/Abilities 폴더)
            LoadDirectory<AbilityDef>(Path.Combine(baseDataPath, "Abilities"), jsonOptions);

            logger.LogInformation("데이터 로딩 완료 (직업: {RoleCount}개, 능력: {AbilityCount}개)", 
                DefDatabase<RoleDef>.Count, 
                DefDatabase<AbilityDef>.Count);
        }

        private void LoadDirectory<T>(string targetDirectory, JsonSerializerOptions options) where T : Def
        {
            if (!Directory.Exists(targetDirectory)) return;

            // 하위 폴더까지 포함해서 모든 .json 파일 검색
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
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "파일 로드 실패: {FileName}", Path.GetFileName(file));
                }
            }
        }
}