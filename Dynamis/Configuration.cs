using Dalamud.Configuration;
using System;
using System.Collections.Generic;

namespace Dynamis;

[Serializable]
public class Configuration : IPluginConfiguration
{

    public Dictionary<uint, List<List<string>>> Mapping { get; set; } = new();

    public Dictionary<string, List<string>> Packages = new();

    public Guid C;

    public int Version { get; set; } = 0;

    public void Save() => Plugin.PluginInterface.SavePluginConfig(this);
}
