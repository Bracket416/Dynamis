using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using static Dalamud.Interface.Utility.Raii.ImRaii;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace Dynamis.Windows;
public class MainWindow : Window, IDisposable
{

    public Dictionary<uint, uint> Actions;
    private Plugin P;
    private static readonly Lumina.Excel.ExcelSheet<Lumina.Excel.Sheets.Action> Reference = Plugin.DataManager.GetExcelSheet<Lumina.Excel.Sheets.Action>();

    private string Action_Name = "";

    private string Package_Name = "";

    public static IPluginLog Log = null!;

    public static string Collection_ID = "";


    private static Dictionary<string, List<string>> Packages = new();

    public static Dictionary<string, List<string>> Packages_Reference = new();

    private static Dictionary<uint, List<List<string>>> Mapping = new();

    public static Dictionary<uint, List<List<string>>> Mapping_Reference = new();

    public static Dictionary<string, int> Levels = new();

    private static readonly List<string> Numbers = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    public MainWindow(Plugin P)
        : base("Dynamis##Main", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 330),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        this.P = P;

    }

    public static uint Get_ID(string Name)
    {
        uint ID = 0;
        for (uint I = 0; I < Reference.Count; I++) if (Reference.GetRow(I).Name == Name) { ID = I; break; }
        return ID;
    }

    public static string Get_Name(uint ID)
    {
        if (Plugin.DataManager.GetExcelSheet<Lumina.Excel.Sheets.Action>().TryGetRow(ID, out var N)) return N.Name.ExtractText();
        return "";
    }

    public void Copy_Reference()
    {
        Mapping.Clear();
        foreach (var Key in Mapping_Reference.Keys) Mapping.Add(Key, Mapping_Reference[Key]);
        Packages.Clear();
        foreach (var Key in Packages_Reference.Keys) Packages.Add(Key, Packages_Reference[Key]);

    }
    private void Update_Reference()
    {
        Mapping_Reference.Clear();
        foreach (var Key in Mapping.Keys) Mapping_Reference.Add(Key, Mapping[Key]);
        Packages_Reference.Clear();
        foreach (var Key in Packages.Keys) Packages_Reference.Add(Key, Packages[Key]);

    }

    public void Dispose() { }

    public override void Draw()
    {
        using (var child = ImRaii.Child("Scroll", Vector2.Zero, true))
        {
            if (child.Success)
            {

                var Add = false;
                ImGui.TextUnformatted("Collection:");
                ImGui.InputTextWithHint("##Collection ID", "Collection ID", ref Collection_ID, 36);
                ImGui.TextUnformatted("————————————————————————————————————————————————————————————————————————————");
                ImGui.TextUnformatted("Packages:");

                foreach (var Key in Packages.Keys)
                {
                    if (Packages[Key].Count == 0)
                    {
                        Packages.Remove(Key);
                        continue;
                    }
                    ImGui.Indent();
                    ImGui.TextUnformatted(Key + ":");
                    ImGui.Indent();
                    for (int I = 0; I < Packages[Key].Count; I++)
                    {
                        if (Levels.Keys.Contains(Key))
                        {
                            ImGui.TextUnformatted((Math.Max(0, (Math.Min(Packages[Key].Count - 1, Levels[Key]))) == I ? $"({I})" : I) + ":");
                        }
                        else ImGui.TextUnformatted(I + ":");

                        ImGui.SameLine();
                        var Text = Packages[Key][I];
                        ImGui.InputTextWithHint("##Package " + Key + " " + I, "Mod", ref Text, 64);
                        Packages[Key][I] = Text;

                    }
                    ImGui.Unindent();
                    ImGui.Spacing();
                    ImGui.Checkbox("+##New Package" + Key, ref Add);
                    if (Add) Packages[Key].Add("");
                    Add = false;
                    ImGui.Checkbox("-##Remove Package" + Key, ref Add);
                    if (Add) Packages[Key].RemoveAt(Packages[Key].Count - 1);
                    Add = false;
                    ImGui.Unindent();
                }

                ImGui.Checkbox("+##AddPackage", ref Add);
                ImGui.SameLine();
                ImGui.InputTextWithHint("##Package", "Package name", ref Package_Name, 64);
                if (Add && Package_Name.Length > 0)
                {
                    if (!Packages.Keys.Contains(Package_Name))
                    {
                        Packages.Add(Package_Name, [""]);
                    }
                    else Packages.Remove(Package_Name);
                }
                Add = false;

                ImGui.TextUnformatted("————————————————————————————————————————————————————————————————————————————");
                ImGui.TextUnformatted("Actions:");

                // ImGui.TextUnformatted(Guid.Parse(ID).ToString());
                foreach (var Key in Mapping.Keys)
                {
                    if (Mapping[Key].Count == 0)
                    {
                        Mapping.Remove(Key);
                        continue;
                    }
                    ImGui.Indent();
                    ImGui.TextUnformatted(Get_Name(Key) + ":");
                    for (var I = 0; I < Mapping[Key].Count; I++)
                    {
                        if (Actions.ContainsKey(Key))
                        {
                            if (Actions[Key] == I + 1)
                            {
                                ImGui.TextUnformatted("(" + I + " → " + (int)((I + 1) % Mapping[Key].Count) + "): ");
                            }
                            else ImGui.TextUnformatted(I + " → " + (int)((I + 1) % Mapping[Key].Count) + ": ");
                        }
                        else ImGui.TextUnformatted(I + " → " + (int)((I + 1) % Mapping[Key].Count) + ": ");
                        ImGui.Indent();
                        for (var J = 0; J < Mapping[Key][I].Count; J++)
                        {
                            ImGui.Checkbox("+##Add " + Key + " " + I + " " + J, ref Add);
                            if (Add) Mapping[Key][I].Insert(J + 1, "");
                            Add = false;
                            ImGui.SameLine();
                            ImGui.Checkbox("-##Remove " + Key + " " + I + " " + J, ref Add);
                            if (Add) Mapping[Key][I].RemoveAt(J);
                            if (!Add)
                            {
                                ImGui.SameLine();
                                var Text = Mapping[Key][I][J];
                                ImGui.InputTextWithHint("##Mod " + Key + " " + I + " " + J, "Level change|Duration|Package", ref Text, 64);
                                if (J < Mapping[Key][I].Count) Mapping[Key][I][J] = Text;
                            }
                            Add = false;
                        }
                        //string.Join(", ", Mapping[Key][I])

                        ImGui.Spacing();
                        ImGui.Checkbox("+##New " + Key + " " + I, ref Add);
                        if (Add) Mapping[Key].Insert(I + 1, [""]);
                        Add = false;
                        ImGui.Checkbox("-##Remove " + Key + " " + I, ref Add);
                        if (Add) Mapping[Key].RemoveAt(I);
                        Add = false;
                        ImGui.Unindent();
                    }
                    ImGui.Spacing();
                    ImGui.Unindent();
                }
                ImGui.Checkbox("+##Add", ref Add);
                ImGui.SameLine();
                ImGui.InputTextWithHint("##Action", "Action name", ref Action_Name, 36);
                if (Add && Action_Name.Length > 0)
                {
                    var Action_ID = Numbers.Contains(Action_Name.Substring(0, 1)) ? uint.Parse(Action_Name) : Get_ID(Action_Name);
                    if (Action_ID != 0)
                    {
                        if (Mapping.ContainsKey(Action_ID))
                        {
                            Mapping.Remove(Action_ID);
                        }
                        else Mapping.Add(Action_ID, [[""]]);
                    }

                }
                Add = false;
                ImGui.Spacing();
                ImGui.Checkbox("Save##Save", ref Add);
                if (Add)
                {
                    Update_Reference();
                    P.Configuration.Mapping = Mapping_Reference;
                    P.Configuration.Packages = Packages_Reference;
                    P.Reset();
                    P.Update_Collection(Collection_ID);
                    P.Configuration.C = P.C;
                    P.Configuration.Save();
                }
                Add = false;
            }
        }
    }
}
