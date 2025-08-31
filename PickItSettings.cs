using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using ExileCore2.Shared.Attributes;
using ExileCore2.Shared.Interfaces;
using ExileCore2.Shared.Nodes;
using ImGuiNET;
using Newtonsoft.Json;

namespace PickIt;

public class PickItSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new(false);
    public InventoryRender InventoryRender { get; set; } = new InventoryRender();
    public RangeNode<Vector2> InventoryPos { get; set; } = new(new Vector2(0, 0), Vector2.Zero, new Vector2(4000, 4000));
    public HotkeyNode ProfilerHotkey { get; set; } = Keys.None;
    public HotkeyNode PickUpKey { get; set; } = Keys.F;
    public ToggleNode PickUpWhenInventoryIsFull { get; set; } = new(false);
    public ToggleNode PickUpEverything { get; set; } = new(false);
    public RangeNode<int> ItemPickupRange { get; set; } = new(600, 1, 1000);
    public RangeNode<int> MonsterCheckRange { get; set; } = new(1000, 1, 2500);

    [Menu(null, "In milliseconds")]
    public RangeNode<int> PauseBetweenClicks { get; set; } = new(100, 0, 500);

    public ToggleNode IgnoreMoving { get; set; } = new(false);

    [ConditionalDisplay(nameof(IgnoreMoving), true)]
    public RangeNode<int> ItemDistanceToIgnoreMoving { get; set; } = new(20, 0, 1000);

    [Menu(null, "Auto pick up any hovered items that match configured filters")]
    public ToggleNode AutoClickHoveredLootInRange { get; set; } = new(false);

    public ToggleNode SmoothCursorMovement { get; set; } = new(true);
    public ToggleNode UseInputLock { get; set; } = new(true);

    public ToggleNode LazyLooting { get; set; } = new(false);

    [ConditionalDisplay(nameof(LazyLooting), true)]
    public ToggleNode NoLazyLootingWhileEnemyClose { get; set; } = new(true);

    [ConditionalDisplay(nameof(LazyLooting), true)]
    public HotkeyNode LazyLootingPauseKey { get; set; } = new(Keys.None);

    [Menu(null, "Includes lazy looting as well as manual activation")]
    public ToggleNode NoLootingWhileEnemyClose { get; set; } = new(false);

    public MiscClickableOptions MiscOptions { get; set; } = new();

    [JsonIgnore]
    public TextNode FilterTest { get; set; } = new();

    [Menu("Use a Custom \"\\config\\custom_folder\" folder ")]
    public TextNode CustomConfigDir { get; set; } = new();

    public List<PickitRule> PickitRules = [];

    [Menu(null, "For debugging. Highlights items if they match an existing filter")]
    [JsonIgnore]
    public ToggleNode DebugHighlight { get; set; } = new(false);
    
    [JsonIgnore]
    public FilterNode Filters { get; } = new();
}

[Submenu(CollapsedByDefault = true)]
public class MiscClickableOptions
{
    public RangeNode<int> MiscPickitRange { get; set; } = new(15, 0, 600);
    public ToggleNode ClickChests { get; set; } = new(true);
    public ToggleNode ClickDoors { get; set; } = new(true);
    public ToggleNode ClickZoneTransitions { get; set; } = new(false);
}

[Submenu(RenderMethod = nameof(Render))]
public class FilterNode
{
    public void Render()
    {
        RulesDisplay.DrawSettings();
    }
}

public record PickitRule(string Name, string Location, bool Enabled)
{
    public bool Enabled = Enabled;
}

[Submenu(CollapsedByDefault = false)]
public class InventoryRender
{
    public ToggleNode ShowInventoryView { get; set; } = new(true);
    public ToggleNode IgnoreFullscreenPanels { get; set; } = new ToggleNode(false);
    public ToggleNode IgnoreLargePanels { get; set; } = new ToggleNode(true);
    public ToggleNode IgnoreChatPanel { get; set; } = new ToggleNode(false);
    public ToggleNode IgnoreLeftPanel { get; set; } = new ToggleNode(true);
    public ToggleNode IgnoreRightPanel { get; set; } = new ToggleNode(true);
    public RangeNode<Vector2> Position { get; set; } = new(new Vector2(50f, 50f), Vector2.Zero, new Vector2(100f, 100f));
    public RangeNode<int> BackdropPadding { get; set; } = new(1, 0, 100);
    public RangeNode<int> CellSize { get; set; } = new(20, 1, 100);
    public RangeNode<int> CellSpacing { get; set; } = new(1, 0, 100);
    public RangeNode<int> ItemOutlineWidth { get; set; } = new(1, 0, 100);
    public ColorNode BackgroundColor { get; set; } = new ColorNode(Color.FromArgb(50, 0, 0, 0));
    public ColorNode ItemOutlineColor { get; set; } = new ColorNode(Color.FromArgb(255, 255, 255, 255));
    public ColorNode OccupiedSlotColor { get; set; } = new ColorNode(Color.FromArgb(160,231,56,56));
    public ColorNode UnoccupiedSlotColor { get; set; } = new ColorNode(Color.FromArgb(81, 130, 250, 130));
}