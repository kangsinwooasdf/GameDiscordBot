using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NetCord;
using NetCord.Rest;
using NetCord.Gateway;
using NetCord.Services.ApplicationCommands;

namespace MafiaDiscordBot.Discord;

public class DiscordMessageManager(GatewayClient client)
{
    private readonly GatewayClient _client = client;

    public async Task<RestMessage> SendTextAsync(ulong channelId, string text)
    {
        // return await _client.Rest.SendMessageAsync(channelId, text)
        return await _client.Rest.SendMessageAsync(channelId, new MessageProperties().WithContent(text));
    }

    // todo
    public async Task<RestMessage> SendImageAsync(ulong channelId, string imageUrl)
    {
        return await _client.Rest.SendMessageAsync(channelId, new MessageProperties().WithAttachments([]));
    }

    // 2. Embed 전송
    public async Task<RestMessage> SendEmbedAsync(ulong channelId, string title, string description, Color color)
    {
        var embed = new EmbedProperties()
        {
            Title = title,
            Description = description,
            Color = color
        };

        var messageProps = new MessageProperties()
        {
            Embeds = new List<EmbedProperties> { embed }
        };

        return await _client.Rest.SendMessageAsync(channelId, messageProps);
    }

    public async Task<RestMessage> SendSelectMenuAsync(ulong channelId, string content, string customId,
        string placeholder, Dictionary<string, string> options)
    {
        var menuOptions = options.Select(opt =>
            new StringMenuSelectOptionProperties(label: opt.Value, value: opt.Key)
        ).ToList();

        // Netcord 문법: customId와 options를 생성자로 전달
        var selectMenu = new StringMenuProperties(customId, menuOptions)
        {
            Placeholder = placeholder
        };

        // ★ 해결: 컴포넌트를 IMessageComponentProperties 리스트로 명시적 캐스팅
        var actionRow = new ActionRowProperties(new List<IActionRowComponentProperties>
            {
                selectMenu as IActionRowComponentProperties
            });

        var messageProps = new MessageProperties()
        {
            Content = content,
            Components = new List<ActionRowProperties> { actionRow }
        };

        return await _client.Rest.SendMessageAsync(channelId, messageProps);
    }
    
    // Embed 명령어
    public async Task ReplyEmbedAsync(SlashCommandContext context, string title, string description, Color color)
    {
        var embed = new EmbedProperties()
        {
            Title = title,
            Description = description,
            Color = color
        };

        // 채널 전송용 MessageProperties가 아닌, 응답 수정용 InteractionMessageProperties 사용
        var messageProps = new InteractionMessageProperties()
        {
            Embeds = new List<EmbedProperties> { embed }
        };

        // 명령어를 친 유저에게 직접 응답 결과를 띄워줍니다.
        await context.Interaction.ModifyResponseAsync(x =>
            x.Embeds = messageProps.Embeds);
    }
}
