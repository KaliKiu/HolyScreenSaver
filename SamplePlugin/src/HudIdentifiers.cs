using System.Collections.Generic;

namespace HolyScreenSaver.Data;
    public class HudIdentifiers
    {
        public static readonly Dictionary<string, string> Hotbars = new()
    {
        { "Hotbar 1", "_ActionBar01" },
        { "Hotbar 2", "_ActionBar02" },
        { "Hotbar 3", "_ActionBar03" },
        { "Hotbar 4", "_ActionBar04" },
        { "Hotbar 5", "_ActionBar05" },
        { "Hotbar 6", "_ActionBar06" },
        { "Hotbar 7", "_ActionBar07" },
        { "Hotbar 8", "_ActionBar08" },
        { "Hotbar 9", "_ActionBar09" },
        { "Hotbar 10", "_ActionBar10" },
        { "Cross Hotbar", "_ActionCross" },
        { "Pet Bar", "_ActionContents" },
    };

        public static readonly Dictionary<string, string> Combat = new()
    {
        { "Target Bar", "_TargetInfo" },
        { "Focus Target", "_FocusTargetInfo" },
        { "Party List", "_PartyList" },
        { "Enemy List", "_EnemyList" },
        { "Limit Break", "_LimitBreak" },
        { "Cast Bar", "_CastBar" },
        { "Status Effects (Self)", "_StatusCustomSelf" },
        { "Status Effects (Target)", "_StatusCustomTarget" },
    };

        public static readonly Dictionary<string, string> World = new()
        {
            { "Mini-map", "_NaviMap"
            },
            { "Duty List", "_DutyList"
            },
            { "Main Menu", "_MainCommand"
            },
            { "Currency", "_Money"
            },
            { "Inventory Grid", "_BagWidget"
            },
            { "Experience Bar", "_ExpBar"
            },
        };
}

