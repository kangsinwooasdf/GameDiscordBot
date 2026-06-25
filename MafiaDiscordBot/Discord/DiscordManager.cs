using System.Threading.Tasks;
using NetCord;
using NetCord.Rest;
using NetCord.Gateway;

namespace MafiaDiscordBot.Discord;

public class DiscordManager(GatewayClient client)
{
    // 밤 페이즈: 채널 채팅 금지
    public async Task LockChannelAsync(ulong channelId, ulong everyoneRoleId)
    {
        var overwrite = new PermissionOverwriteProperties(everyoneRoleId, PermissionOverwriteType.Role)
        {
            Allowed = 0,
            Denied = Permissions.SendMessages
        };

        await client.Rest.ModifyGuildChannelPermissionsAsync(channelId, overwrite);
    }

    // 낮 페이즈: 채널 채팅 재허용
    public async Task UnlockChannelAsync(ulong channelId, ulong everyoneRoleId)
    {
        var overwrite = new PermissionOverwriteProperties(everyoneRoleId, PermissionOverwriteType.Role)
        {
            Allowed = Permissions.SendMessages,
            Denied = 0
        };

        await client.Rest.ModifyGuildChannelPermissionsAsync(channelId, overwrite);
    }
}