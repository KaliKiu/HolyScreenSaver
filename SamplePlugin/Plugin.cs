using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using HolyScreenSaver.Data;
using HolyScreenSaver.HudHandler;
using HolyScreenSaver.Windows;
using Lumina.Data.Parsing.Scd;
using System;
using System.IO;
using System.Numerics;
using static FFXIVClientStructs.FFXIV.Client.UI.Agent.AgentMJIFarmManagement;


namespace HolyScreenSaver;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IPlayerState PlayerState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] public static IGameGui GameGui { get; private set; } = null!;
    public HolyScreenSaver.HudHandler.HudHandler HudHandler { get; private set; }

    private const string CommandName = "/pmycommand";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("SamplePlugin");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private IFramework framework;

    private DateTime _lastUpdate = DateTime.MinValue;
    private const float Speed = 5f; // Speed in pixels per update

    public Plugin()
    {
        this.HudHandler = new HolyScreenSaver.HudHandler.HudHandler(GameGui);
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        InitializeHudStates();
        var goatImagePath = Path.Combine(PluginInterface.AssemblyLocation.Directory?.FullName!, "goat.png");

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this, goatImagePath);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = ""
        });

        Vector2? pos = this.HudHandler.GetHotbarPosition("_ActionBar01");
        Log.Debug("Hotbar position: {Position}", pos.HasValue ? pos.Value.ToString() : "null");

        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

        this.framework = Framework;
       
        this.framework.Update += OnUpdate;
    }
    public void InitializeHudStates()
    {
        var cid = PlayerState.ContentId;
        if (cid == 0) return; 
        var stats = Configuration.GetStats(cid);
        var random = new Random();
        stats.SavedHudStates.Clear();

        foreach (var entry in HudIdentifiers.Hotbars)
            {
            var pos = this.HudHandler.GetHotbarPosition(entry.Value);
                if (pos.HasValue)
                {
                double angleDegrees = random.NextDouble() * 360.0;

                float angleRadians = (float)(angleDegrees * (Math.PI / 180.0));

                float velX = Speed * MathF.Cos(angleRadians);
                float velY = Speed * MathF.Sin(angleRadians);

                stats.SavedHudStates.Add(new HudElement
                    {
                        ElementId = entry.Value,
                        FirstPosition = pos.Value,
                        CurrentPosition = pos.Value,
                        Velocity = new Vector2(velX, velY)
                });
                }
            }
        Configuration.Save();
        Log.Info($"Successfully replaced HUD layout for character {cid}");
    }
    public void ResetAllElements()
    {
        var cid = PlayerState.ContentId;
        CharacterStats stats = Configuration.GetStats(cid);

        foreach (var element in stats.SavedHudStates)
        {
            Log.Debug($"Resetting {element.ElementId} to {element.FirstPosition}");
            element.CurrentPosition = element.FirstPosition;
            HudHandler.UpdateElementPosition(element);

        }
    }
    public void OnUpdate(IFramework framework)
    {
        var cid = PlayerState.ContentId;
        CharacterStats stats = Configuration.GetStats(cid);

        if (stats.MoveBool)
        {
            MoveElements();
        }
    }
    private void MoveElements()
    {
        var cid = PlayerState.ContentId;
        CharacterStats stats = Configuration.GetStats(cid);
        foreach(var element in stats.SavedHudStates)
        {
            HudHandler.UpdateElementPosition(element);
            HudHandler.HandleBouncing(element, 1920f, 1080f);
            Log.Debug($"{element.ElementId} is now at {element.CurrentPosition} with velocity {element.Velocity}");
        }
    }
    public void Dispose()
    {
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        MainWindow.Toggle();
    }
    
    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();
}
