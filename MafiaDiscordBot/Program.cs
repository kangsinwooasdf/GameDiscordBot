using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using NetCord;
using NetCord.Rest;
using NetCord.Gateway;
using MafiaDiscordBot.Mafia.Defs;
using NetCord.Services.ApplicationCommands;

namespace MafiaDiscordBot;
internal class Program
{
    static async Task Main(string[] args)
    {
        string botToken = args[0];
        
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // 기본 로깅 설정 추가
                services.AddLogging(configure => configure.AddConsole());
                    
                // DefLoader를 컨테이너에 등록
                services.AddTransient<DefLoader>();
                services.AddSingleton<ApplicationCommandService<SlashCommandContext>>();

                // 봇 클라이언트를 싱글톤으로 등록
                services.AddSingleton(new GatewayClient(new BotToken(botToken), new GatewayClientConfiguration
                {
                    Intents = GatewayIntents.GuildMessages | GatewayIntents.MessageContent
                }));
            });

        var host = builder.Build();
        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        // 2. 봇 구동 전, 데이터를 먼저 메모리에 로드
        using (var scope = host.Services.CreateScope())
        {
            var defLoader = scope.ServiceProvider.GetRequiredService<DefLoader>();
            defLoader.LoadAll(); // 여기서 JSON 파싱 실행
        }

        // 3. 디스코드 봇 이벤트 설정 및 실행
        var client = host.Services.GetRequiredService<GatewayClient>();
        var commandService = host.Services.GetRequiredService<ApplicationCommandService<SlashCommandContext>>();
            
        commandService.AddModules(Assembly.GetEntryAssembly()!);

        // ★ 3. 디스코드에서 유저가 빗금(/) 명령어를 쳤을 때 발생하는 이벤트 연결
        client.InteractionCreate += async interaction =>
        {
            if (interaction is SlashCommandInteraction slashCommand)
            {
                try
                {
                    // 엔진이 알아서 알맞은 메서드(ReloadDataAsync 등)를 찾아 실행해 줍니다.
                    await commandService.ExecuteAsync(new SlashCommandContext(slashCommand, client));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "명령어 실행 중 오류 발생");
                }
            }
        };
        
        logger.LogInformation("마피아 봇을 시작합니다...");

        await client.StartAsync();
        
        client.Ready += async (ReadyEventArgs _) =>
        {
            logger.LogInformation("슬래시 명령어를 디스코드에 동기화합니다...");
            await commandService.RegisterCommandsAsync(client.Rest, client.Id);
            logger.LogInformation("동기화 완료!");
        };
        
        await host.RunAsync();
    }
}