using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ExileCore2.Shared.Attributes;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
using ImGuiNET;
using Newtonsoft.Json;

namespace PickIt;

public class PickItSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new ToggleNode(false);
    public ToggleNode ShowInventoryView { get; set; } = new ToggleNode(true);
    public ToggleNode MoveInventoryView { get; set; } = new ToggleNode(false);
    public HotkeyNode ProfilerHotkey { get; set; } = Keys.None;
    public HotkeyNode PickUpKey { get; set; } = Keys.F;
    public ToggleNode PickUpWhenInventoryIsFull { get; set; } = new ToggleNode(false);
    public RangeNode<int> PickupRange { get; set; } = new RangeNode<int>(600, 1, 1000);
    public ToggleNode IgnoreMoving { get; set; } = new ToggleNode(false);
    public RangeNode<int> ItemDistanceToIgnoreMoving { get; set; } = new RangeNode<int>(20, 0, 1000);
    public RangeNode<int> PauseBetweenClicks { get; set; } = new RangeNode<int>(100, 0, 500);
    public ToggleNode HoverClickLooting { get; set; } = new ToggleNode(false);
    public ToggleNode LazyLooting { get; set; } = new ToggleNode(false);
    public ToggleNode NoLazyLootingWhileEnemyClose { get; set; } = new ToggleNode(false);
    public HotkeyNode LazyLootingPauseKey { get; set; } = new HotkeyNode(Keys.Space);
    public ToggleNode PickUpEverything { get; set; } = new ToggleNode(false);
    public ToggleNode ClickChests { get; set; } = new ToggleNode(true);
    public ToggleNode ClickQuestChests { get; set; } = new ToggleNode(true);
    public ToggleNode ItemizeCorpses { get; set; } = new ToggleNode(true);

    [JsonIgnore]
    public TextNode FilterTest { get; set; } = new TextNode();

    [JsonIgnore]
    public ButtonNode ReloadFilters { get; set; } = new ButtonNode();

    [Menu("Use a Custom \"\\config\\custom_folder\" folder ")]
    public TextNode CustomConfigDir { get; set; } = new TextNode();

    public List<PickitRule> PickitRules = new List<PickitRule>();

    [JsonIgnore]
    public FilterNode Filters { get; } = new FilterNode();

    [JsonIgnore]
    public ToggleNode DebugHighlight { get; set; } = new ToggleNode(false);
}

[Submenu(RenderMethod = nameof(Render))]
public class FilterNode
{
    public void Render(PickIt pickit)
    {
        if (ImGui.Button("Open filter Folder"))
        {
            var configDir = pickit.ConfigDirectory;
            var customConfigFileDirectory = !string.IsNullOrEmpty(pickit.Settings.CustomConfigDir)
                ? Path.Combine(Path.GetDirectoryName(pickit.ConfigDirectory), pickit.Settings.CustomConfigDir)
                : null;

            var directoryToOpen = Directory.Exists(customConfigFileDirectory)
                ? customConfigFileDirectory
                : configDir;

            Process.Start("explorer.exe", directoryToOpen);
        }

        ImGui.Separator();
        ImGui.BulletText("Select Rules To Load");
        ImGui.BulletText("Ordering rule sets so general items will match first rather than last will improve performance");

        var tempNpcInvRules = new List<PickitRule>(pickit.Settings.PickitRules); // Create a copy

        for (int i = 0; i < tempNpcInvRules.Count; i++)
        {
            ImGui.PushID(i);
            if (ImGui.ArrowButton("##upButton", ImGuiDir.Up) && i > 0)
                (tempNpcInvRules[i - 1], tempNpcInvRules[i]) = (tempNpcInvRules[i], tempNpcInvRules[i - 1]);

            ImGui.SameLine();
            ImGui.Text(" ");
            ImGui.SameLine();

            if (ImGui.ArrowButton("##downButton", ImGuiDir.Down) && i < tempNpcInvRules.Count - 1)
                (tempNpcInvRules[i + 1], tempNpcInvRules[i]) = (tempNpcInvRules[i], tempNpcInvRules[i + 1]);

            ImGui.SameLine();
            ImGui.Text(" - ");
            ImGui.SameLine();

            ImGui.Checkbox($"{tempNpcInvRules[i].Name}###enabled", ref tempNpcInvRules[i].Enabled);
            ImGui.PopID();
        }

        pickit.Settings.PickitRules = tempNpcInvRules;
    }
}

public record PickitRule(string Name, string Location, bool Enabled)
{
    public bool Enabled = Enabled;
}