using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using MafiaDiscordBot.Discord;
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
                // 로깅 설정
                services.AddLogging(configure => configure.AddConsole());
                    
                // DefLoader를 컨테이너에 등록
                services.AddTransient<DefLoader>();

                // 싱글톤
                services.AddSingleton(new GatewayClient(new BotToken(botToken), new GatewayClientConfiguration
                {
                    Intents = GatewayIntents.GuildMessages | GatewayIntents.MessageContent
                }));

                services.AddSingleton<ApplicationCommandService<SlashCommandContext>>();
                services.AddSingleton<DiscordMessageManager>();
                services.AddSingleton<DiscordManager>();
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
                // 1. 매 명령어 실행마다 DI 스코프(재료 상자) 생성
                using var scope = host.Services.CreateScope();
                try
                {
                    // 2. 엔진에 Context와 함께 'scope.ServiceProvider'를 꼭 같이 넘겨주어야 DefLoader가 주입됩니다!
                    var result = await commandService.ExecuteAsync(new SlashCommandContext(slashCommand, client), scope.ServiceProvider);
                    
                    // 3. Exception이 아닌, 시스템적 거부(권한 부족, DI 실패 등)가 발생했을 때 로그 띄우기
                    if (result is NetCord.Services.IFailResult failResult)
                    {
                        logger.LogWarning("명령어 실행 거부됨: {Reason}", failResult.Message);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "명령어 실행 중 치명적 오류 발생");
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