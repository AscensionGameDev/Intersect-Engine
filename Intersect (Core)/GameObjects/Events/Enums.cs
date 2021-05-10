namespace Intersect.GameObjects.Events
{

    public enum MoveRouteEnum
    {

        MoveUp = 1,

        MoveDown,

        MoveLeft,

        MoveRight,

        MoveRandomly,

        MoveTowardsPlayer,

        MoveAwayFromPlayer,

        StepForward,

        StepBack,

        FaceUp,

        FaceDown,

        FaceLeft,

        FaceRight,

        Turn90Clockwise,

        Turn90CounterClockwise,

        Turn180,

        TurnRandomly,

        FacePlayer,

        FaceAwayFromPlayer,

        SetSpeedSlowest,

        SetSpeedSlower,

        SetSpeedNormal,

        SetSpeedFaster,

        SetSpeedFastest,

        SetFreqLowest,

        SetFreqLower,

        SetFreqNormal,

        SetFreqHigher,

        SetFreqHighest,

        WalkingAnimOn,

        WalkingAnimOff,

        DirectionFixOn,

        DirectionFixOff,

        WalkthroughOn,

        WalkthroughOff,

        ShowName,

        HideName,

        SetLevelBelow,

        SetLevelNormal,

        SetLevelAbove,

        Wait100,

        Wait500,

        Wait1000,

        SetGraphic,

        SetAnimation,

    }

    //ONLY ADD TO THE END OF THIS LIST ELSE FACE THE WRATH OF JC!!!!!
    public enum EventCommandType
    {

        Null = 0,

        //Dialog
        ShowText,

        ShowOptions,

        AddChatboxText,

        //Logic Flow
        SetVariable = 5,

        SetSelfSwitch,

        ConditionalBranch,

        ExitEventProcess,

        Label,

        GoToLabel,

        StartCommonEvent,

        //Player Control
        RestoreHp,

        RestoreMp,

        LevelUp,

        GiveExperience,

        ChangeLevel,

        ChangeSpells,

        ChangeItems,

        ChangeSprite,

        ChangeFace,

        ChangeGender,

        SetAccess,

        //Movement,
        WarpPlayer,

        SetMoveRoute,

        WaitForRouteCompletion,

        HoldPlayer,

        ReleasePlayer,

        SpawnNpc,

        //Special Effects
        PlayAnimation,

        PlayBgm,

        FadeoutBgm,

        PlaySound,

        StopSounds,

        //Etc
        Wait,

        //Shop and Bank
        OpenBank,

        OpenShop,

        OpenCraftingTable,

        //Extras
        SetClass,

        DespawnNpc,

        //Questing
        StartQuest,

        CompleteQuestTask,

        EndQuest,

        //Pictures
        ShowPicture,

        HidePicture,

        //Hide/show player
        HidePlayer,

        ShowPlayer,

        //Equip Items
        EquipItem,

        //Change Name Color
        ChangeNameColor,

        //Player Input variable.
        InputVariable,

        //Player Label
        PlayerLabel,

        // Player Color
        ChangePlayerColor,

        ChangeName,

        //Guilds
        CreateGuild,
        DisbandGuild,
        OpenGuildBank,
        SetGuildBankSlots
        //End Guilds

    }

}
