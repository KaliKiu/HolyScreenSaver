using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Windowing;
using HolyScreenSaver;
using Serilog;

namespace HolyScreenSaver.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;
    private readonly Plugin plugin;

    // We give this window a constant ID using ###.
    // This allows for labels to be dynamic, like "{FPS Counter}fps###XYZ counter window",
    // and the window ID will always be "###XYZ counter window" for ImGui
    public ConfigWindow(Plugin plugin) : base("A Wonderful Configuration Window###With a constant ID")
    {
        Flags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar |
                ImGuiWindowFlags.NoScrollWithMouse;

        Size = new Vector2(400, 400);
        SizeCondition = ImGuiCond.Always;

        configuration = plugin.Configuration;
        this.plugin = plugin;
    }

    public void Dispose() { }

    public override void PreDraw()
    {
        if (configuration.IsConfigWindowMovable)
        {
            Flags &= ~ImGuiWindowFlags.NoMove;
        }
        else
        {
            Flags |= ImGuiWindowFlags.NoMove;
        }
    }

    public override void Draw()
    {
        var cid = Plugin.PlayerState.ContentId;
        var stats = configuration.GetStats(cid);

        if(ImGui.Button("GetPos"))
        {
            Log.Debug("Pressed get position button");
            Vector2? hotbarPos = this.plugin.HudHandler.GetHotbarPosition("_ActionBar01");
            if (hotbarPos.HasValue)
            {
                float x = hotbarPos.Value.X;
                float y = hotbarPos.Value.Y;

                Log.Debug($"Hotbar 1 is at: {x}, {y}");
            }
            else
            {
                Log.Debug("Could not find Hotbar 1. Is it hidden?");
            }
        }
        if (ImGui.Button("SetPos"))
        {
            Log.Debug("Pressed set position button");
            this.plugin.HudHandler.SetHotbarPosition("_ActionBar01", new Vector2(678, 956));
        }
        var movebool = stats.MoveBool;
        if (ImGui.Checkbox("Gambling Mode", ref movebool))
        {
            stats.MoveBool = movebool;
            configuration.Save();
        }
        if (ImGui.Button("SaveHudLayout"))
        {
            Log.Debug("Pressed save HUD layout button");
            this.plugin.InitializeHudStates();
        }
        if (ImGui.Button("RESETALL"))
        {
            Log.Debug("Pressed save HUD layout button");
            this.plugin.ResetAllElements();
            configuration.Save();
        }
    }
}
