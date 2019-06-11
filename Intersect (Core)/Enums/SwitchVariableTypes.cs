namespace Intersect.Enums
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

    public enum VariableDataTypes : byte
    {
        Boolean = 1,
        Integer,
        Number,
        String
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
        Random,
        SystemTime,
        DupPlayerVar,
        DupGlobalVar,
        AddPlayerVar,
        AddGlobalVar,
        SubtractPlayerVar,
        SubtractGlobalVar,
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

    public enum VariableCompareTypes
    {
        StaticValue = 0,
        PlayerVariable,
        GlobalVariable
    }
}