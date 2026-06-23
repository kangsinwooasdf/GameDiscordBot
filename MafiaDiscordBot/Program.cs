using NetCord;
using NetCord.Gateway;

namespace MafiaDiscordBot;
internal class Program
{
    static async Task Main(string[] args)
    {
        string botToken = args[0];
        
        // 봇 클라이언트 생성 (인텐트 설정 포함)
        var client = new GatewayClient(new BotToken(botToken), new GatewayClientConfiguration
        {
            Intents = GatewayIntents.GuildMessages | GatewayIntents.MessageContent
        });

        // 메시지 수신 이벤트 핸들러 등록
        client.MessageCreate += async message =>
        {
            if (message.Author.IsBot) return;

            if (message.Content == "!ping")
            {
                await message.ReplyAsync("Pong!");
            }
        };

        // 봇 실행
        await client.StartAsync();
        await Task.Delay(-1);
    }
}