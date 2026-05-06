using Dalamud.Configuration;
using System;
using System.Collections.Generic;
using HolyScreenSaver.Data;

namespace HolyScreenSaver;

public class CharacterStats
{
    public List<HudElement> SavedHudStates { get; set; } = new();
    public bool MoveBool { get; set; } = false;
}
[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;
    public bool IsConfigWindowMovable { get; set; } = true;
    public bool SomePropertyToBeSavedAndWithADefault { get; set; } = true;

    public readonly Dictionary<ulong, CharacterStats> CharacterData = new();

    public CharacterStats GetStats(ulong cid)
    {
        if (!CharacterData.ContainsKey(cid))
            CharacterData[cid] = new CharacterStats();

        return CharacterData[cid];
    }
    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
