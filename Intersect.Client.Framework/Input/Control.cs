namespace Intersect.Client.Framework.Input;

public enum Control
{
    MoveUp,

    MoveDown,

    MoveLeft,

    MoveRight,

    AttackInteract,

    Block,

    AutoTarget,

    HoldToSoftRetargetOnSelfCast,

    ToggleAutoSoftRetargetOnSelfCast,

    PickUp,

    Enter,

    Screenshot,

    OpenMenu,

    OpenInventory,

    OpenQuests,

    OpenCharacterInfo,

    OpenParties,

    OpenSpells,

    OpenFriends,

    OpenGuild,

    OpenSettings,

    OpenDebugger,

    OpenAdminPanel,

    ToggleGui,

    TurnAround,

    ToggleZoomIn,

    ToggleZoomOut,

    HoldToZoomIn,

    HoldToZoomOut,

    ToggleFullscreen,

    // Add new controls above this line
    // If new controls are added that are similar to hotkeys (in that they map to some configurable slot count)
    // they should have an explicit offset like hotkeys, and the offset should be >0x1000 and far enough away from
    // other offset ranges that they won't collide under reasonable circumstances.

    HotkeyOffset = 0x1000,

    Hotkey1,

    Hotkey2,

    Hotkey3,

    Hotkey4,

    Hotkey5,

    Hotkey6,

    Hotkey7,

    Hotkey8,

    Hotkey9,

    Hotkey10,
}

// TODO: Switch to a record struct (incomplete) that can be used for controls that can be added via plugin (or at least not in this file)
/*
public record struct Control(string Name)
   {
       public static readonly Control MoveUp = new(nameof(MoveUp));
   	public static readonly Control MoveLeft = new(nameof(MoveLeft));
   	public static readonly Control MoveDown = new(nameof(MoveDown));
   	public static readonly Control MoveRight = new(nameof(MoveRight));
   	public static readonly Control AttackInteract = new(nameof(AttackInteract));
   	public static readonly Control Block = new(nameof(Block));
   	public static readonly Control AutoTarget = new(nameof(AutoTarget));
   	public static readonly Control HoldToSoftRetargetOnSelfCast = new(nameof(HoldToSoftRetargetOnSelfCast));
   	public static readonly Control ToggleAutoSoftRetargetOnSelfCast = new(nameof(ToggleAutoSoftRetargetOnSelfCast));
   	public static readonly Control PickUp = new(nameof(PickUp));
   	public static readonly Control Enter = new(nameof(Enter));
   	public static readonly Control Screenshot = new(nameof(Screenshot));
   	public static readonly Control OpenMenu = new(nameof(OpenMenu));
   	public static readonly Control OpenInventory = new(nameof(OpenInventory));
   	public static readonly Control OpenQuests = new(nameof(OpenQuests));
   	public static readonly Control OpenCharacterInfo = new(nameof(OpenCharacterInfo));
   	public static readonly Control OpenParties = new(nameof(OpenParties));
   	public static readonly Control OpenSpells = new(nameof(OpenSpells));
   	public static readonly Control OpenGuild = new(nameof(OpenGuild));
   	public static readonly Control OpenFriends = new(nameof(OpenFriends));
   	public static readonly Control OpenSettings = new(nameof(OpenSettings));
   	public static readonly Control OpenDebugger = new(nameof(OpenDebugger));
   	public static readonly Control OpenAdminPanel = new(nameof(OpenAdminPanel));
   	public static readonly Control ToggleGui = new(nameof(ToggleGui));
   	public static readonly Control TurnAround = new(nameof(TurnAround));
   	public static readonly Control ToggleZoomIn = new(nameof(ToggleZoomIn));
   	public static readonly Control HoldToZoomIn = new(nameof(HoldToZoomIn));
   	public static readonly Control ToggleZoomOut = new(nameof(ToggleZoomOut));
   	public static readonly Control HoldToZoomOut = new(nameof(HoldToZoomOut));
   	public static readonly Control ToggleFullscreen = new(nameof(ToggleFullscreen));
   	public static readonly Control Hotkey = new(nameof(Hotkey));
   }
*/