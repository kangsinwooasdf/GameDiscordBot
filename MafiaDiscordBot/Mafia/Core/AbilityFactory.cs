using System.Reflection;
using MafiaDiscordBot.Debug;
using MafiaDiscordBot.Mafia.Defs;
using MafiaDiscordBot.Mafia.Game.Abilities;

namespace MafiaDiscordBot.Mafia.Core;

public class AbilityFactory
{
    public static List<IAbility> CreateAbilityList(List<string> abilityDefNames)
    {
        var abilities = new List<IAbility>();

        foreach (var defName in abilityDefNames)
        {
            var def = DefDatabase<AbilityDef>.Get(defName);
            if (def == null)
            {
                Console.WriteLine($"{defName} 능력을 찾을 수 없음");
                continue;
            }

            var type = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => typeof(IAbility).IsAssignableFrom(t) && t.Name == def.ClassName);

            if (type == null)
            {
                // todo: Console.WriteLine 부분 전부다 Log 클래스에 위임해야할 것
                Console.WriteLine($"{def.ClassName} C# 클래스를 찾을 수 없음");
                continue;
            }

            if (Activator.CreateInstance(type, def) is IAbility abilityInstance)
            {
                abilities.Add(abilityInstance);
            }
        }

        return abilities;
    }
}