using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCord;
using NetCord.Gateway;
using MafiaDiscordBot.Mafia.Defs;

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
            
        client.MessageCreate += async message =>
        {
            if (message.Author.IsBot) return;
            if (message.Content == "!ping")
            {
                await message.ReplyAsync("Pong!");
            }
        };

        logger.LogInformation("마피아 봇을 시작합니다...");

        await client.StartAsync();
        await host.RunAsync();
    }
}