namespace Intersect.Migration.UpgradeInstructions.Upgrade_12.Intersect_Convert_Lib.Enums
{
    public enum SwitchVariableTypes
    {
        PlayerSwitch = 0,
        PlayerVariable,
        ServerSwitch,
        ServerVariable,
    }


    public enum SwitchTypes
    {
        PlayerSwitch = 0,
        ServerSwitch
    }

    public enum VariableTypes
    {
        PlayerVariable = 0,
        ServerVariable
    }

    public enum VariableMods
    {
        Set = 0,
        Add,
        Subtract,
        Random
    }

    public enum VariableComparators
    {
        Equal = 0,
        GreaterOrEqual,
        LesserOrEqual,
        Greater,
        Less,
        NotEqual
    }
}