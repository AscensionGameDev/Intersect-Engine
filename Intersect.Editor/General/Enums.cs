﻿namespace Intersect.Editor.General
{

    public enum EditorTypes
    {

        Animation = 0,

        Item = 1,

        Npc = 2,

        Resource = 3,

        Spell = 4,

        Class = 5,

        Quest = 6,

        Projectile = 7,

        Event = 8,

        CommonEvent = 9,

        SwitchVariable = 10,

        Shop = 11,

    }

    public enum MapListUpdates
    {

        MoveItem = 0,

        AddFolder = 1,

        Rename = 2,

        Delete = 3,

    }

    public enum EditingTool
    {

        Brush = 0,

        MarqueeSelection = 1,

        RectangleFill = 2,

        LayerFill = 3,

        Eraser = 4,

        Eyedropper = 5,

    }

    public enum SelectionTypes
    {

        AllLayers = 0,

        CurrentLayer = 1,

    }

}
