using Dalamud.Hooking;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Dynamis.Windows;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Common.Lua;
using Penumbra.Api.Enums;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Delegates;

namespace Dynamis.Handler
{
    public static unsafe class Hook
    {
        public static Handler.Action* Manager;

        public static Dictionary<string, int> Levels = new();
        public static Dictionary<uint, uint> Actions = new();
        private static List<ValueTuple<string, double, int>> Queues = new();

        public static Dictionary<uint, List<List<string>>> Mapping = new();
        public static Dictionary<string, List<string>> Packages = new();

        public static IClientState Client;
        private delegate void ReceiveActionEffectDelegate(int sourceActorID, nint sourceActor, nint vectorPosition, nint effectHeader, nint effectArray, nint effectTrail);
        private static Hook<ReceiveActionEffectDelegate> ReceiveActionEffectHook;
        private static uint Map_ID = 0;
        public static IPluginLog Log = null!;

        public static Func<string, bool, PenumbraApiEc> Setter;
        private static void ReceiveActionEffectDetour(int sourceActorID, nint sourceActor, nint vectorPosition, nint effectHeader, nint effectArray, nint effectTrail)
        {
            var ID = *(ushort*)(effectHeader + 0x1C);
            Log.Information($"Entity {sourceActorID} used " + MainWindow.Get_Name(ID) + $" ({ID}).");
            if (Client.LocalPlayer != null)
            {
                if (sourceActorID == Client.LocalPlayer.EntityId)
                {
                    if (Setter != null)
                    {
                        var Index = -1;
                        if (Mapping.ContainsKey(ID))
                        {
                            if (!Actions.ContainsKey(ID)) Actions.Add(ID, 0);
                            Actions[ID]++;
                            if (Actions[ID] > Mapping[ID].Count) Actions[ID] = 1;
                            Index = (int)Math.Max(0, Math.Min(Actions[ID] - 1, Mapping[ID].Count));
                        }
                        if (Index > -1 && Index < Mapping[ID].Count) foreach (var Order in Mapping[ID][Index])
                            {
                                var Split = Order.Split("|");
                                var Upgrade = 1;
                                var Level = Split[0];
                                int.TryParse(Level, out Upgrade);
                                var Time = 0.0;
                                Double.TryParse(Split[1], out Time);
                                Update_Package(Split[2], Time + 0.1, Upgrade, false);
                            }
                    }
                }
            }
            ReceiveActionEffectHook.Original(sourceActorID, sourceActor, vectorPosition, effectHeader, effectArray, effectTrail);
        }

        private static void Update_Package(string Package, double Time, int Upgrade, bool Instant)
        {
            if (Packages.ContainsKey(Package))
            {
                if (double.IsNegativeInfinity(Time))
                {
                    if (!Levels.ContainsKey(Package)) Levels.Add(Package, 0);
                    Levels[Package] += Upgrade;
                    var Index = Math.Max(0, Math.Min(Packages[Package].Count - 1, Levels[Package]));
                    Log.Information($"Level of {Package}: " + Levels[Package]);
                    foreach (var Part in Packages[Package]) if (Part != Packages[Package][Index] && Part.Length != 0) Setter(Part, false);
                    if (Packages[Package][Index].Length != 0) Setter(Packages[Package][Index], true);

                }
                else
                {
                    Queues.Add(ValueTuple.Create(Package, Instant ? 0.0 : 0.1, Upgrade));
                    if (Time > 0.1) Queues.Add(ValueTuple.Create(Package, Time + 0.1, -Upgrade));
                }
            }
        }
        public static void Initialize()
        {
            Manager = (Handler.Action*)FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance();
            ReceiveActionEffectHook = DalamudApi.GameInteropProvider.HookFromAddress<ReceiveActionEffectDelegate>(DalamudApi.SigScanner.ScanModule("40 55 56 57 41 54 41 55 41 56 41 57 48 8D AC 24"), ReceiveActionEffectDetour);
            ReceiveActionEffectHook.Enable();
            DalamudApi.Framework.Update += Update;
        }

        public static void Reset()
        {
            Levels.Clear();
            Actions.Clear();
            var Index = 0;
            foreach (var Package in Packages.Keys)
            {
                Levels.Add(Package, 0);
                if (Packages[Package].Count == 0) continue;
                foreach (var Part in Packages[Package]) if (Part != Packages[Package][Index] && Part.Length != 0) Setter(Part, false);
                if (Packages[Package][Index].Length != 0) Setter(Packages[Package][Index], true);
            }
        }

        private static Dictionary<string, int> Gauges = new();

        private static string All_Jobs = "PLD WAR DRK GNB MNK DRG NIN SAM RPR VPR WHM SCH AST SGE BRD MCH DNC BLM SMN RDM PCT";
        private static void Update(IFramework F)
        {
            if (Client.LocalPlayer != null)
            {
                foreach (var Key in Levels.Keys) if (All_Jobs.Contains(Key.Split(" ")[0]))
                    {
                        var New_Level = Jobs.Get_Gauge(Key);
                        if (!Gauges.ContainsKey(Key)) Gauges.Add(Key, New_Level);
                        var Delta = New_Level - Gauges[Key];
                        if (Delta != 0) Update_Package(Key, 0.1, Delta, true);
                        Gauges[Key] = New_Level;
                    }
                var Removed = new List<int>();
                for (var I = 0; I < Queues.Count; I++)
                {
                    Queues[I] = ValueTuple.Create(Queues[I].Item1, Queues[I].Item2 - F.UpdateDelta.TotalSeconds, Queues[I].Item3);
                    if (Queues[I].Item2 <= 0)
                    {
                        Update_Package(Queues[I].Item1, double.NegativeInfinity, Queues[I].Item3, true);
                        Removed.Add(I);
                    }
                }
                Removed.Reverse();
                foreach (var I in Removed) Queues.RemoveAt(I);
                if (Client.LocalPlayer != null)
                {
                    if (Client.LocalPlayer.IsDead || Map_ID != Client.MapId) Reset();
                    Map_ID = Client.MapId;
                }
            }
        }

        public static void Dispose()
        {
            ReceiveActionEffectHook?.Dispose();
        }
    }

}
