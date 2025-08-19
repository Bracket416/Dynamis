using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Plugin;
using System.IO;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dynamis.Windows;
using Dynamis.Handler;
using System.Collections.Generic;
using Penumbra.Api;
using Penumbra.Api.Api;
using Penumbra.Api.Enums;
using Penumbra.Api.Helpers;
using Penumbra.Api.IpcSubscribers;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using FFXIVClientStructs.FFXIV.Common.Lua;

namespace Dynamis;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;

    private const string CommandName = "/dynamis";

    private Dictionary<uint, uint> Actions = new();

    private TrySetMod Setter;

    public Guid C;

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("Dynamis");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }

    public void Reset()
    {
        Hook.Reset();
    }

    public PenumbraApiEc Set(string Path, bool State)
    {
        var Split_Path = Path.Split("/");
        return Setter.Invoke(C, string.Join("/", Split_Path.SkipLast(1)), State, Split_Path[^1]);
    }

    public void Update_Collection(string ID) => Guid.TryParse(ID, out C);

    public Plugin(IDalamudPluginInterface I)
    {
        
        DalamudApi.Initialize(this, I);
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        // You might normally want to embed resources and load them from the manifest stream
        C = Configuration.C;
        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);
        MainWindow.Actions = Actions;
        Setter = new TrySetMod(I);
        MainWindow.Log = Log;
        MainWindow.Collection_ID = C.ToString();
        Hook.Log = Log;
        Hook.Setter = Set;
        Hook.Actions = Actions;
        Hook.Client = ClientState;
        Hook.Initialize();
        Hook.Mapping = Configuration.Mapping;
        Hook.Packages = Configuration.Packages;
        MainWindow.Mapping_Reference = Hook.Mapping;
        MainWindow.Packages_Reference = Hook.Packages;
        MainWindow.Levels = Hook.Levels;
        MainWindow.Copy_Reference();
        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);
        
        CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        PluginInterface.UiBuilder.Draw += DrawUI;

        // This adds a button to the plugin installer entry of this plugin which allows
        // toggling the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUI;

        // Adds another button doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;

        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        // Example Output: 00:57:54.959 | INF | [Dynamis] ===A cool log message from Sample Plugin===
        //Log.Information($"===A cool log message from {PluginInterface.Manifest.Name}===");
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();
        Hook.Dispose();
        DalamudApi.Dispose();

        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();

    public void ToggleConfigUI() => ConfigWindow.Toggle();
    public void ToggleMainUI() => MainWindow.Toggle();
}
