namespace Flex.LVA.Shared
{
    public enum LogElementType
    {
        Unknown = 0,
        String = 1,
        Number = 2,
        DateTime = 3,
        Time = 4,
        Date = 5,
        Bool = 6,
    }

    public enum LogSyntaxType
    {
        Parent = 1,
        Child = 2,
    }

    public enum LogDataType
    {
        PlainText = 1,
        Xml = 2,
        Json = 3,
        Evtx = 4,
        Auto = 5,
        Delimited = 6
    }
}