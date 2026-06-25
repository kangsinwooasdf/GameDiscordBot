namespace MafiaDiscordBot.Mafia.Defs;

public static class DefDatabase
{
    public static readonly Dictionary<Type, string> DefRegistry = new()
    {
        { typeof(RoleDef), "Jobs" },
        { typeof(AbilityDef), "Abilities" },
        { typeof(MessageDef), "Message" }
    };
}

public static class DefDatabase<T> where T : Def
{
    // DefName(예: "Role_Mafia")을 키값으로 사용하는 딕셔너리
    private static readonly Dictionary<string, T> _defs = new();

    // 데이터 등록
    public static void Add(T def)
    {
        if (!string.IsNullOrEmpty(def.DefName))
        {
            _defs[def.DefName] = def;
        }
    }

    // 데이터 검색 (아이디로 찾기)
    public static T? Get(string defName)
    {
        return _defs.GetValueOrDefault(defName);
    }

    // 등록된 모든 데이터 반환
    public static IEnumerable<T> AllDefs => _defs.Values;

    // 디버깅 및 초기화용
    public static void Clear() => _defs.Clear();
    public static int Count => _defs.Count;
}